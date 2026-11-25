using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NadinShop.Application.Common.Services;
using NadinShop.Infrastructure;
using NadinShop.Infrastructure.Data;
using NadinShop.Infrastructure.Services;

namespace NadinShop.Infrastructure.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddInfrastructure_RegistersServiceImplementations()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = "Server=(localdb)\\MSSQLLocalDB;Database=NadinShopTests;Trusted_Connection=True",
                ["Jwt:Issuer"] = "NadinShop.Tests",
                ["Jwt:Audience"] = "NadinShop.Tests.Client",
                ["Jwt:SigningKey"] = "nadin-shop-tests-signing-key-with-enough-bytes-1234567890",
                ["Jwt:AccessTokenMinutes"] = "30"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        using var scope = provider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        Assert.IsType<CurrentUserService>(scopedProvider.GetRequiredService<ICurrentUserService>());
        Assert.IsType<IdentityService>(scopedProvider.GetRequiredService<IIdentityService>());
        Assert.IsType<JwtTokenService>(scopedProvider.GetRequiredService<IJwTokenService>());
        Assert.NotNull(scopedProvider.GetRequiredService<NadinShopDbContext>());
    }

    [Fact]
    public void AddInfrastructure_WithoutJwtConfiguration_Throws()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = "Server=(localdb)\\MSSQLLocalDB;Database=NadinShopTests;Trusted_Connection=True"
            })
            .Build();

        var services = new ServiceCollection();

        Assert.Throws<InvalidOperationException>(() => services.AddInfrastructure(configuration));
    }
}
