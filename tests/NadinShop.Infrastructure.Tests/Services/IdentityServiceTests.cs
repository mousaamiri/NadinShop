using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Infrastructure.Data.Entities;
using NadinShop.Infrastructure.Services;
using NadinShop.Infrastructure.Tests.TestSupport;

namespace NadinShop.Infrastructure.Tests.Services;

public class IdentityServiceTests
{
    private const string Email = "user@example.com";
    private const string Phone = "09121234567";
    private const string Password = "P@ssw0rd!";

    private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

    [Fact]
    public async Task CreateUserAsync_WithNewUser_PersistsNormalizedUserWithHashedPassword()
    {
        using var dbContext = TestDatabase.Create();
        var service = CreateService(dbContext);

        var result = await service.CreateUserAsync("  User@Example.COM ", "0912 123 4567", Password, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        var user = await dbContext.Set<ApplicationUser>().SingleAsync();
        Assert.Equal(result.Value, user.Id);
        Assert.Equal("User@Example.COM", user.Email);
        Assert.Equal("user@example.com", user.NormalizedEmail);
        Assert.Equal("0912 123 4567", user.Phone);
        Assert.Equal("09121234567", user.NormalizedPhone);
        Assert.NotEqual(Password, user.PasswordHash);
        Assert.Equal(
            PasswordVerificationResult.Success,
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Password));
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingEmailIgnoringCase_ReturnsConflict()
    {
        using var dbContext = TestDatabase.Create();
        var service = CreateService(dbContext);
        await service.CreateUserAsync(Email, Phone, Password, CancellationToken.None);

        var result = await service.CreateUserAsync("USER@EXAMPLE.COM", "09351234567", Password, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Error.EmailAlreadyExists", result.Error.Code);
        Assert.Equal(ErrorType.Conflict, result.Error.ErrorType);
        Assert.Equal(1, await dbContext.Set<ApplicationUser>().CountAsync());
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingPhoneIgnoringSeparators_ReturnsConflict()
    {
        using var dbContext = TestDatabase.Create();
        var service = CreateService(dbContext);
        await service.CreateUserAsync(Email, Phone, Password, CancellationToken.None);

        var result = await service.CreateUserAsync("other@example.com", "0912-123-4567", Password, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Error.PhoneAlreadyExists", result.Error.Code);
        Assert.Equal(ErrorType.Conflict, result.Error.ErrorType);
        Assert.Equal(1, await dbContext.Set<ApplicationUser>().CountAsync());
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
        var service = CreateService(dbContext);

        var result = await service.CreateUserAsync(email, phone, password, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(expectedErrorCode, result.Error.Code);
        Assert.Equal(ErrorType.Validation, result.Error.ErrorType);
        Assert.Empty(dbContext.Set<ApplicationUser>());
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithCorrectPassword_ReturnsAuthUser()
    {
        using var dbContext = TestDatabase.Create();
        var service = CreateService(dbContext);
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
        var service = CreateService(dbContext);
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
        var service = CreateService(dbContext);
        await service.CreateUserAsync(Email, Phone, Password, CancellationToken.None);

        var unknownUserResult = await service.ValidateCredentialsAsync("missing@example.com", Password, CancellationToken.None);
        var wrongPasswordResult = await service.ValidateCredentialsAsync(Email, "wrong-password", CancellationToken.None);

        Assert.True(unknownUserResult.IsFailure);
        Assert.Equal(wrongPasswordResult.Error, unknownUserResult.Error);
    }

    private IdentityService CreateService(Data.NadinShopDbContext dbContext)
        => new(dbContext, _passwordHasher);
}
