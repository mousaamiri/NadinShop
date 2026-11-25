using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NadinShop.Application.Common.Services;
using NadinShop.Infrastructure.Data;

namespace NadinShop.Infrastructure.Services;

public sealed class CurrentUserService(
    IHttpContextAccessor httpContextAccessor,
    NadinShopDbContext dbContext) : ICurrentUserService
{
    private const string SubjectClaimType = "sub";

    public Task<Guid?> GetCurrentUserIdAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var principal = httpContextAccessor.HttpContext?.User;
        var rawUserId = principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? principal?.FindFirstValue(SubjectClaimType);

        return Task.FromResult(Guid.TryParse(rawUserId, out var userId) ? userId : (Guid?)null);
    }

    public async Task<CurrentUser?> GetCurrentUserAsync(CancellationToken ct)
    {
        var userId = await GetCurrentUserIdAsync(ct);
        if (userId is null)
            return null;

        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(candidate => candidate.Id == userId.Value, ct);

        return user is null
            ? null
            : new CurrentUser(user.Id, user.Email ?? string.Empty, user.PhoneNumber ?? string.Empty);
    }
}
