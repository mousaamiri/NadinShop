using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NadinShop.Application.Features.Products.Queries.Get;
using NadinShop.IntegrationTests.TestSupport;
using NadinShop.Infrastructure.Data;

namespace NadinShop.IntegrationTests;

public class DependencyInjectionTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public void DbContext_UsesInMemoryProvider()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NadinShopDbContext>();

        Assert.Equal("Microsoft.EntityFrameworkCore.InMemory", dbContext.Database.ProviderName);
    }

    [Fact]
    public async Task Mediator_CanHandleGetProductsQuery()
    {
        using var scope = factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new GetProductsQuery());

        Assert.True(result.IsSuccess);
    }
}
