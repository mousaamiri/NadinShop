using NadinShop.Application.Common.Interfaces;

namespace NadinShop.Infrastructure.Data;

public class UnitOfWork(NadinShopDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
