using Microsoft.EntityFrameworkCore;
using NadinShop.Domain.Entities.Products;
using NadinShop.Infrastructure.Data.Entities;

namespace NadinShop.Infrastructure.Data;

public class NadinShopDbContext(DbContextOptions<NadinShopDbContext> dbContextOptions):DbContext(dbContextOptions)
{
    internal DbSet<Product> Products => Set<Product>();
    internal DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NadinShopDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
