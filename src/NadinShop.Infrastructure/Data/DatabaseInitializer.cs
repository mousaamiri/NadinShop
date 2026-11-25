using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NadinShop.Infrastructure.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(services);

        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NadinShopDbContext>();

        if (!dbContext.Database.IsRelational())
        {
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            return;
        }

        if (!dbContext.Database.GetMigrations().Any())
        {
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            return;
        }

        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
