using NadinShop.Application.Features.Users.Commands.Dtos;

namespace NadinShop.Application.Common.Services;

public interface IJwTokenService
{
    Task<JwtResult> GenerateAccessToken(AuthUser authUser, CancellationToken ct);
    string GenerateRefreshToken();
}
