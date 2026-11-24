namespace NadinShop.Application.Common.Services;

public interface ICurrentUserService
{
    Task<Guid?> GetCurrentUserIdAsync(CancellationToken ct);
}
