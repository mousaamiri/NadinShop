using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NadinShop.Application.Features.Identity.Commands.Dtos;
using NadinShop.Infrastructure.Services;
using NadinShop.Infrastructure.Tests.TestSupport;

namespace NadinShop.Infrastructure.Tests.Services;

public class JwtTokenServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 24, 10, 30, 0, TimeSpan.Zero);

    private static readonly JwtOptions Options = new()
    {
        Issuer = "NadinShop.Tests",
        Audience = "NadinShop.Tests.Client",
        SigningKey = "nadin-shop-tests-signing-key-with-enough-bytes-1234567890",
        AccessTokenMinutes = 30
    };

    private static JwtTokenService CreateService(JwtOptions? options = null)
        => new(options ?? Options, new FakeTimeProvider(Now));

    [Fact]
    public async Task GenerateAccessToken_WithValidUser_ReturnsSignedTokenWithUserClaims()
    {
        var user = new AuthUser(Guid.NewGuid(), "user@example.com");
        var service = CreateService();

        var result = await service.GenerateAccessToken(user, CancellationToken.None);

        Assert.Equal(user.UserId.ToString(), result.UserId);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.Equal(Now.AddMinutes(Options.AccessTokenMinutes).UtcDateTime, result.ExpirationDate);
        Assert.Equal(DateTimeKind.Utc, result.ExpirationDate.Kind);

        var principal = await ValidateAsync(result.AccessToken);

        Assert.Equal(user.UserId.ToString(), ReadSubject(principal));
        Assert.Equal("user@example.com", principal.FindFirst("email")?.Value);
        Assert.Equal(Options.Issuer, principal.FindFirst("iss")?.Value);
    }

    [Fact]
    public async Task GenerateAccessToken_CalledTwice_ReturnsDifferentTokens()
    {
        var user = new AuthUser(Guid.NewGuid(), "user@example.com");
        var service = CreateService();

        var first = await service.GenerateAccessToken(user, CancellationToken.None);
        var second = await service.GenerateAccessToken(user, CancellationToken.None);

        Assert.NotEqual(first.AccessToken, second.AccessToken);
        Assert.NotEqual(first.RefreshToken, second.RefreshToken);
    }

    [Fact]
    public async Task GenerateAccessToken_WithCanceledToken_Throws()
    {
        var service = CreateService();
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.GenerateAccessToken(new AuthUser(Guid.NewGuid(), "user@example.com"), cancellationTokenSource.Token));
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsBase64UrlEncodedRandomValue()
    {
        var service = CreateService();

        var first = service.GenerateRefreshToken();
        var second = service.GenerateRefreshToken();

        Assert.NotEqual(first, second);
        Assert.Equal(64, Base64UrlEncoder.DecodeBytes(first).Length);
    }

    [Theory]
    [InlineData("")]
    [InlineData("too-short")]
    public void Constructor_WithInvalidSigningKey_Throws(string signingKey)
    {
        var options = new JwtOptions
        {
            Issuer = Options.Issuer,
            Audience = Options.Audience,
            SigningKey = signingKey,
            AccessTokenMinutes = 30
        };

        Assert.Throws<InvalidOperationException>(() => CreateService(options));
    }

    [Fact]
    public void Constructor_WithMissingIssuer_Throws()
    {
        var options = new JwtOptions
        {
            Audience = Options.Audience,
            SigningKey = Options.SigningKey,
            AccessTokenMinutes = 30
        };

        Assert.Throws<InvalidOperationException>(() => CreateService(options));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_WithNonPositiveAccessTokenLifetime_Throws(int accessTokenMinutes)
    {
        var options = new JwtOptions
        {
            Issuer = Options.Issuer,
            Audience = Options.Audience,
            SigningKey = Options.SigningKey,
            AccessTokenMinutes = accessTokenMinutes
        };

        Assert.Throws<InvalidOperationException>(() => CreateService(options));
    }

    private static string? ReadSubject(ClaimsPrincipal principal)
        => principal.FindFirst("sub")?.Value ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    private static async Task<ClaimsPrincipal> ValidateAsync(string accessToken)
    {
        var handler = new JsonWebTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = Options.Issuer,
            ValidAudience = Options.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.SigningKey)),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false
        };

        var validationResult = await handler.ValidateTokenAsync(accessToken, validationParameters);

        Assert.True(validationResult.IsValid, validationResult.Exception?.Message);
        return new ClaimsPrincipal(validationResult.ClaimsIdentity);
    }
}
