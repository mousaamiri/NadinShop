using NadinShop.Domain.Entities.Products;
using NadinShop.Infrastructure.Repositories;
using NadinShop.Infrastructure.Tests.TestSupport;

namespace NadinShop.Infrastructure.Tests.Repositories;

public class ProductRepositoryTests
{
    [Fact]
    public async Task GetByNameAsync_WithExistingName_ReturnsProduct()
    {
        using var dbContext = TestDatabase.Create();
        var repository = new ProductRepository(dbContext);
        var product = Product.Create("Laptop Pro", "09121234567", "manufacturer@example.com", isAvailable: true);
        repository.Add(product);
        await dbContext.SaveChangesAsync();

        var result = await repository.GetByNameAsync("Laptop Pro");

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
    }

    [Fact]
    public async Task GetByNameAsync_WithUnknownName_ReturnsNull()
    {
        using var dbContext = TestDatabase.Create();
        var repository = new ProductRepository(dbContext);

        var result = await repository.GetByNameAsync("Missing Product");

        Assert.Null(result);
    }

    [Fact]
    public async Task ListByManufactureEmailAsync_WithMatchingEmail_ReturnsOnlyThatManufacturerProducts()
    {
        using var dbContext = TestDatabase.Create();
        var repository = new ProductRepository(dbContext);
        var matching = Product.Create("Laptop Pro", "09121234567", "manufacturer@example.com", isAvailable: true);
        var other = Product.Create("Monitor Wide", "09121234567", "other@example.com", isAvailable: true);
        repository.Add(matching);
        repository.Add(other);
        await dbContext.SaveChangesAsync();

        var result = await repository.ListByManufactureEmailAsync("  MANUFACTURER@Example.COM ");

        var product = Assert.Single(result);
        Assert.Equal(matching.Id, product.Id);
    }

    [Fact]
    public async Task ListByManufactureEmailAsync_WithUnknownEmail_ReturnsEmptyList()
    {
        using var dbContext = TestDatabase.Create();
        var repository = new ProductRepository(dbContext);
        repository.Add(Product.Create("Laptop Pro", "09121234567", "manufacturer@example.com", isAvailable: true));
        await dbContext.SaveChangesAsync();

        var result = await repository.ListByManufactureEmailAsync("missing@example.com");

        Assert.Empty(result);
    }
}
