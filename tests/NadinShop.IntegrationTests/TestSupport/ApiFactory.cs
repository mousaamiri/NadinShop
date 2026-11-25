using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NadinShop.IntegrationTests.TestControllers;
using NadinShop.Infrastructure.Data;

namespace NadinShop.IntegrationTests.TestSupport;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"NadinShopIntegrationTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging => logging.ClearProviders());

        builder.ConfigureServices(services =>
        {
            var contextRegistrations = services
                .Where(descriptor => IsDbContextRegistration(descriptor.ServiceType))
                .ToList();

            foreach (var registration in contextRegistrations)
                services.Remove(registration);

            services.AddDbContext<NadinShopDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.AddControllers()
                .AddApplicationPart(typeof(TestErrorsController).Assembly);
        });
    }

    private static bool IsDbContextRegistration(Type serviceType)
    {
        if (serviceType == typeof(NadinShopDbContext)
            || serviceType == typeof(DbContextOptions)
            || serviceType == typeof(DbContextOptions<NadinShopDbContext>))
        {
            return true;
        }

        return serviceType.IsGenericType
               && serviceType.Name.StartsWith("IDbContextOptionsConfiguration", StringComparison.Ordinal)
               && serviceType.GetGenericArguments()[0] == typeof(NadinShopDbContext);
    }
}
