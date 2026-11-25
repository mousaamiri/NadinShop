using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NadinShop.Domain.Entities.Products;
using NadinShop.Infrastructure.Data.Entities;

namespace NadinShop.Infrastructure.Data;

public class NadinShopDbContext(DbContextOptions<NadinShopDbContext> dbContextOptions)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(dbContextOptions)
{
    internal DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NadinShopDbContext).Assembly);
    }
}
