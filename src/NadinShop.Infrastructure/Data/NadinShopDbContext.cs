using Microsoft.EntityFrameworkCore;
using NadinShop.Domain.Entities.Products;

namespace NadinShop.Infrastructure.Data;

public class NadinShopDbContext(DbContextOptions<NadinShopDbContext> dbContextOptions):DbContext(dbContextOptions)
{
    internal DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NadinShopDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
