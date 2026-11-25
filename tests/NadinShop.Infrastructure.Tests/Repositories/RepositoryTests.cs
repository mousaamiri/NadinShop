using Microsoft.EntityFrameworkCore;
using NadinShop.Domain.Entities.Products;
using NadinShop.Infrastructure.Data;
using NadinShop.Infrastructure.Repositories;
using NadinShop.Infrastructure.Tests.TestSupport;

namespace NadinShop.Infrastructure.Tests.Repositories;

public class RepositoryTests
{
    [Fact]
    public async Task Add_ThenSaveChanges_PersistsEntity()
    {
        using var dbContext = TestDatabase.Create();
        var repository = new Repository<Product, Guid>(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var product = CreateProduct("Laptop Pro");

        repository.Add(product);
        await unitOfWork.SaveChangesAsync();

        var stored = await dbContext.Set<Product>().SingleAsync();
        Assert.Equal(product.Id, stored.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsEntity()
    {
        using var dbContext = TestDatabase.Create();
        var repository = new Repository<Product, Guid>(dbContext);
        var product = CreateProduct("Laptop Pro");
        repository.Add(product);
        await dbContext.SaveChangesAsync();

        var result = await repository.GetByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithUnknownId_ReturnsNull()
    {
        using var dbContext = TestDatabase.Create();
        var repository = new Repository<Product, Guid>(dbContext);

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task ListAsync_ReturnsAllEntities()
    {
        using var dbContext = TestDatabase.Create();
        var repository = new Repository<Product, Guid>(dbContext);
        repository.Add(CreateProduct("Laptop Pro"));
        repository.Add(CreateProduct("Monitor Wide"));
        await dbContext.SaveChangesAsync();

        var result = await repository.ListAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        using var dbContext = TestDatabase.Create();
        var repository = new Repository<Product, Guid>(dbContext);
        var product = CreateProduct("Laptop Pro");
        repository.Add(product);
        await dbContext.SaveChangesAsync();

        Assert.True(await repository.ExistsAsync(product.Id));
        Assert.False(await repository.ExistsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Remove_ThenSaveChanges_DeletesEntity()
    {
        using var dbContext = TestDatabase.Create();
        var repository = new Repository<Product, Guid>(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var product = CreateProduct("Laptop Pro");
        repository.Add(product);
        await unitOfWork.SaveChangesAsync();

        repository.Remove(product);
        await unitOfWork.SaveChangesAsync();

        Assert.Empty(await dbContext.Set<Product>().ToListAsync());
    }

    private static Product CreateProduct(string name)
    {
        return Product.Create(name, "09121234567", "manufacturer@example.com", isAvailable: true);
    }
}
