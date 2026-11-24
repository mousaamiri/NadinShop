using NadinShop.Domain.Entities.Products;

namespace NadinShop.Application.Common.Interfaces;

public interface IProductRepository : IRepository<Product, Guid>
{
    Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
