using NadinShop.Domain.Entities.Products;
using NadinShop.Infrastructure.Data;
using NadinShop.Infrastructure.Tests.TestSupport;

namespace NadinShop.Infrastructure.Tests.Persistence;

public class UnitOfWorkTests
{
    [Fact]
    public async Task SaveChangesAsync_WithPendingChanges_ReturnsAffectedRows()
    {
        using var dbContext = TestDatabase.Create();
        var unitOfWork = new UnitOfWork(dbContext);
        dbContext.Set<Product>().Add(
            Product.Create("Laptop Pro", "09121234567", "manufacturer@example.com", isAvailable: true));

        var affectedRows = await unitOfWork.SaveChangesAsync();

        Assert.Equal(1, affectedRows);
    }

    [Fact]
    public async Task SaveChangesAsync_WithoutPendingChanges_ReturnsZero()
    {
        using var dbContext = TestDatabase.Create();
        var unitOfWork = new UnitOfWork(dbContext);

        var affectedRows = await unitOfWork.SaveChangesAsync();

        Assert.Equal(0, affectedRows);
    }
}
