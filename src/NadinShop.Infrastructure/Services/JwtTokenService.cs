using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Identity.Commands.Dtos;

namespace NadinShop.Infrastructure.Services;

public sealed class JwtTokenService : IJwTokenService
{
    private const string SubjectClaimType = "sub";
    private const string EmailClaimType = "email";
    private const string JwtIdClaimType = "jti";
    private const int RefreshTokenSizeInBytes = 64;

    private readonly JwtOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly SigningCredentials _signingCredentials;
    private readonly JsonWebTokenHandler _tokenHandler = new();

    public JwtTokenService(JwtOptions options, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(timeProvider);

        options.Validate();

        _options = options;
        _timeProvider = timeProvider;
        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
            SecurityAlgorithms.HmacSha256);
    }

    public Task<JwtResult> GenerateAccessToken(AuthUser authUser, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(authUser);
        ct.ThrowIfCancellationRequested();

        var issuedAt = _timeProvider.GetUtcNow();
        var expiresAt = issuedAt.AddMinutes(_options.AccessTokenMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = issuedAt.UtcDateTime,
            NotBefore = issuedAt.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = _signingCredentials,
            Claims = new Dictionary<string, object>
            {
                [SubjectClaimType] = authUser.UserId.ToString(),
                [EmailClaimType] = authUser.Email,
                [JwtIdClaimType] = Guid.NewGuid().ToString()
            }
        };

        var accessToken = _tokenHandler.CreateToken(descriptor);

        return Task.FromResult(new JwtResult(
            authUser.UserId.ToString(),
            accessToken,
            GenerateRefreshToken(),
            expiresAt.UtcDateTime));
    }

    public string GenerateRefreshToken()
        => Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(RefreshTokenSizeInBytes));
}
