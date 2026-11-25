using Microsoft.EntityFrameworkCore;
using NadinShop.Infrastructure.Data;

namespace NadinShop.Infrastructure.Tests.TestSupport;

public static class TestDatabase
{
    public static NadinShopDbContext Create()
    {
        var options = new DbContextOptionsBuilder<NadinShopDbContext>()
            .UseInMemoryDatabase($"NadinShopTests-{Guid.NewGuid()}")
            .Options;

        return new NadinShopDbContext(options);
    }
}
