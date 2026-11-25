using Microsoft.EntityFrameworkCore;
using NadinShop.Domain.Entities.Products;
using NadinShop.Domain.Entities.Products.ValueObjects;
using NadinShop.Infrastructure.Data;

namespace NadinShop.Infrastructure.Tests.Repositories;

public class ProductQueryTranslationTests
{
    [Fact]
    public void ExistsByManufactureEmailAndProduceDateQuery_TranslatesToSql()
    {
        var options = new DbContextOptionsBuilder<NadinShopDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=NadinShopTranslationTests;Trusted_Connection=True")
            .Options;

        using var context = new NadinShopDbContext(options);
        var email = ManufactureEmail.Create("probe@example.com");
        var date = ProduceDate.Create(new DateTime(2024, 8, 1));

        var sql = context.Set<Product>()
            .Where(product => product.ManufactureEmail.Equals(email) && product.ProduceDate.Equals(date))
            .ToQueryString();

        Assert.Contains("[ManufactureEmail]", sql, StringComparison.Ordinal);
        Assert.Contains("[ProduceDate]", sql, StringComparison.Ordinal);
    }
}
