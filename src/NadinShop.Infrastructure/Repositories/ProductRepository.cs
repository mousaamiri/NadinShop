using Microsoft.EntityFrameworkCore;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Domain.Entities.Products;
using NadinShop.Domain.Entities.Products.ValueObjects;
using NadinShop.Infrastructure.Data;

namespace NadinShop.Infrastructure.Repositories;

public class ProductRepository(NadinShopDbContext context) : Repository<Product, Guid>(context), IProductRepository
{
    public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var productName = ProductName.Create(name);

        return await Set.FirstOrDefaultAsync(
            product => product.Name.Equals(productName),
            cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListByManufactureEmailAsync(
        string manufactureEmail,
        CancellationToken cancellationToken = default)
    {
        var email = ManufactureEmail.Create(manufactureEmail);

        return await Set
            .Where(product => product.ManufactureEmail.Equals(email))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByManufactureEmailAndProduceDateAsync(
        string manufactureEmail,
        DateTime produceDate,
        CancellationToken cancellationToken = default)
    {
        var email = ManufactureEmail.Create(manufactureEmail);
        var date = ProduceDate.Create(produceDate);

        return await Set.AnyAsync(
            product => product.ManufactureEmail.Equals(email) && product.ProduceDate.Equals(date),
            cancellationToken);
    }
}
