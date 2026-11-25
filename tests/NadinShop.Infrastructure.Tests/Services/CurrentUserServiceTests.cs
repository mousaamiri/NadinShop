using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NadinShop.Infrastructure.Data.Entities;
using NadinShop.Infrastructure.Services;
using NadinShop.Infrastructure.Tests.TestSupport;

namespace NadinShop.Infrastructure.Tests.Services;

public class CurrentUserServiceTests
{
    [Fact]
    public async Task GetCurrentUserIdAsync_WithoutHttpContext_ReturnsNull()
    {
        using var dbContext = TestDatabase.Create();
        var service = new CurrentUserService(new HttpContextAccessor(), dbContext);

        var userId = await service.GetCurrentUserIdAsync(CancellationToken.None);

        Assert.Null(userId);
    }

    [Fact]
    public async Task GetCurrentUserIdAsync_WithSubjectClaim_ReturnsUserId()
    {
        using var dbContext = TestDatabase.Create();
        var expectedUserId = Guid.NewGuid();
        var service = CreateService(dbContext, new Claim("sub", expectedUserId.ToString()));

        var userId = await service.GetCurrentUserIdAsync(CancellationToken.None);

        Assert.Equal(expectedUserId, userId);
    }

    [Fact]
    public async Task GetCurrentUserIdAsync_WithNameIdentifierClaim_ReturnsUserId()
    {
        using var dbContext = TestDatabase.Create();
        var expectedUserId = Guid.NewGuid();
        var service = CreateService(dbContext, new Claim(ClaimTypes.NameIdentifier, expectedUserId.ToString()));

        var userId = await service.GetCurrentUserIdAsync(CancellationToken.None);

        Assert.Equal(expectedUserId, userId);
    }

    [Theory]
    [InlineData("not-a-guid")]
    [InlineData("")]
    public async Task GetCurrentUserIdAsync_WithInvalidClaim_ReturnsNull(string claimValue)
    {
        using var dbContext = TestDatabase.Create();
        var service = CreateService(dbContext, new Claim("sub", claimValue));

        var userId = await service.GetCurrentUserIdAsync(CancellationToken.None);

        Assert.Null(userId);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithKnownUser_ReturnsUserDetails()
    {
        using var dbContext = TestDatabase.Create();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "user@example.com",
            NormalizedUserName = "USER@EXAMPLE.COM",
            Email = "user@example.com",
            NormalizedEmail = "user@example.com",
            PhoneNumber = "09121234567",
            NormalizedPhone = "09121234567",
            PasswordHash = "hash",
            CreatedAtUtc = DateTime.UtcNow
        };
        dbContext.Set<ApplicationUser>().Add(user);
        await dbContext.SaveChangesAsync();
        var service = CreateService(dbContext, new Claim("sub", user.Id.ToString()));

        var currentUser = await service.GetCurrentUserAsync(CancellationToken.None);

        Assert.NotNull(currentUser);
        Assert.Equal(user.Id, currentUser.UserId);
        Assert.Equal(user.Email, currentUser.Email);
        Assert.Equal(user.PhoneNumber, currentUser.Phone);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithUnknownUser_ReturnsNull()
    {
        using var dbContext = TestDatabase.Create();
        var service = CreateService(dbContext, new Claim("sub", Guid.NewGuid().ToString()));

        var currentUser = await service.GetCurrentUserAsync(CancellationToken.None);

        Assert.Null(currentUser);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithoutClaims_ReturnsNull()
    {
        using var dbContext = TestDatabase.Create();
        var service = new CurrentUserService(new HttpContextAccessor(), dbContext);

        var currentUser = await service.GetCurrentUserAsync(CancellationToken.None);

        Assert.Null(currentUser);
    }

    private static CurrentUserService CreateService(Data.NadinShopDbContext dbContext, params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "Test");
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        var accessor = new HttpContextAccessor { HttpContext = httpContext };

        return new CurrentUserService(accessor, dbContext);
    }
}
