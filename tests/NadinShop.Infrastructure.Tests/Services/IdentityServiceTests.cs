using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Infrastructure.Data;
using NadinShop.Infrastructure.Data.Entities;
using NadinShop.Infrastructure.Services;
using NadinShop.Infrastructure.Tests.TestSupport;

namespace NadinShop.Infrastructure.Tests.Services;

public class IdentityServiceTests
{
    private const string Email = "user@example.com";
    private const string Phone = "09121234567";
    private const string Password = "P@ssw0rd!";

    [Fact]
    public async Task CreateUserAsync_WithNewUser_PersistsNormalizedUserWithHashedPassword()
    {
        using var dbContext = TestDatabase.Create();
        var (service, userManager) = CreateService(dbContext);

        var result = await service.CreateUserAsync(
            "  User@Example.COM ",
            "0912 123 4567",
            Password,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        var user = await dbContext.Users.SingleAsync();
        Assert.Equal(result.Value, user.Id);
        Assert.Equal("User@Example.COM", user.Email);
        Assert.Equal("USER@EXAMPLE.COM", user.NormalizedEmail);
        Assert.Equal("User@Example.COM", user.UserName);
        Assert.Equal("0912 123 4567", user.PhoneNumber);
        Assert.Equal("09121234567", user.NormalizedPhone);
        Assert.NotEqual(Password, user.PasswordHash);
        Assert.True(await userManager.CheckPasswordAsync(user, Password));
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingEmailIgnoringCase_ReturnsConflict()
    {
        using var dbContext = TestDatabase.Create();
        var (service, _) = CreateService(dbContext);
        await service.CreateUserAsync(Email, Phone, Password, CancellationToken.None);

        var result = await service.CreateUserAsync(
            "USER@EXAMPLE.COM",
            "09351234567",
            Password,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Error.EmailAlreadyExists", result.Error.Code);
        Assert.Equal(ErrorType.Conflict, result.Error.ErrorType);
        Assert.Equal(1, await dbContext.Users.CountAsync());
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingPhoneIgnoringSeparators_ReturnsConflict()
    {
        using var dbContext = TestDatabase.Create();
        var (service, _) = CreateService(dbContext);
        await service.CreateUserAsync(Email, Phone, Password, CancellationToken.None);

        var result = await service.CreateUserAsync(
            "other@example.com",
            "0912-123-4567",
            Password,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Error.PhoneAlreadyExists", result.Error.Code);
        Assert.Equal(ErrorType.Conflict, result.Error.ErrorType);
        Assert.Equal(1, await dbContext.Users.CountAsync());
    }

    [Theory]
    [InlineData("", Phone, Password, "Error.EmailRequired")]
    [InlineData(Email, "   ", Password, "Error.PhoneRequired")]
    [InlineData(Email, Phone, "", "Error.PasswordRequired")]
    public async Task CreateUserAsync_WithMissingRequiredValue_ReturnsValidationError(
        string email,
        string phone,
        string password,
        string expectedErrorCode)
    {
        using var dbContext = TestDatabase.Create();
        var (service, _) = CreateService(dbContext);

        var result = await service.CreateUserAsync(email, phone, password, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(expectedErrorCode, result.Error.Code);
        Assert.Equal(ErrorType.Validation, result.Error.ErrorType);
        Assert.Empty(dbContext.Users);
    }

    [Fact]
    public async Task CreateUserAsync_WithPasswordRejectedByIdentity_ReturnsValidationError()
    {
        using var dbContext = TestDatabase.Create();
        var (service, _) = CreateService(dbContext);

        var result = await service.CreateUserAsync(Email, Phone, "weak", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Error.UserCreationFailed", result.Error.Code);
        Assert.Equal(ErrorType.Validation, result.Error.ErrorType);
        Assert.Empty(dbContext.Users);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithCorrectPassword_ReturnsAuthUser()
    {
        using var dbContext = TestDatabase.Create();
        var (service, _) = CreateService(dbContext);
        var createdUserId = (await service.CreateUserAsync(Email, Phone, Password, CancellationToken.None)).Value;

        var result = await service.ValidateCredentialsAsync("USER@example.com", Password, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(createdUserId, result.Value.UserId);
        Assert.Equal(Email, result.Value.Email);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithWrongPassword_ReturnsUnauthorized()
    {
        using var dbContext = TestDatabase.Create();
        var (service, _) = CreateService(dbContext);
        await service.CreateUserAsync(Email, Phone, Password, CancellationToken.None);

        var result = await service.ValidateCredentialsAsync(Email, "wrong-password", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Error.InvalidCredentials", result.Error.Code);
        Assert.Equal(ErrorType.Unauthorized, result.Error.ErrorType);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithUnknownEmail_ReturnsSameUnauthorizedError()
    {
        using var dbContext = TestDatabase.Create();
        var (service, _) = CreateService(dbContext);
        await service.CreateUserAsync(Email, Phone, Password, CancellationToken.None);

        var unknownUserResult = await service.ValidateCredentialsAsync(
            "missing@example.com",
            Password,
            CancellationToken.None);
        var wrongPasswordResult = await service.ValidateCredentialsAsync(
            Email,
            "wrong-password",
            CancellationToken.None);

        Assert.True(unknownUserResult.IsFailure);
        Assert.Equal(wrongPasswordResult.Error, unknownUserResult.Error);
    }

    private static (IdentityService Service, UserManager<ApplicationUser> UserManager) CreateService(
        NadinShopDbContext dbContext)
    {
        var options = Options.Create(new IdentityOptions());
        options.Value.Password.RequiredLength = 8;
        options.Value.Password.RequireDigit = true;
        options.Value.Password.RequireLowercase = true;
        options.Value.Password.RequireUppercase = true;
        options.Value.Password.RequireNonAlphanumeric = true;

        var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser, IdentityRole<Guid>, NadinShopDbContext, Guid>(dbContext),
            options,
            new PasswordHasher<ApplicationUser>(),
            [new UserValidator<ApplicationUser>()],
            [new PasswordValidator<ApplicationUser>()],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            services: null!,
            logger: NullLogger<UserManager<ApplicationUser>>.Instance);

        return (new IdentityService(userManager), userManager);
    }
}
