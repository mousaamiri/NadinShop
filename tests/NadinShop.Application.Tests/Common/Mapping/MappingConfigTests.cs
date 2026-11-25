using Mapster;
using NadinShop.Application.Common.Mapping;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Identity.Commands.Dtos;
using NadinShop.Application.Features.Products.Commands.Create;
using NadinShop.Application.Features.Products.Commands.Update;
using NadinShop.Application.Features.Products.Dtos;
using NadinShop.Domain.Entities.Products;

namespace NadinShop.Application.Tests.Common.Mapping;

public class MappingConfigTests
{
    [Fact]
    public void Adapt_ProductToCreateProductResponse_MapsAllValues()
    {
        var product = Product.Create("Galaxy S24", "09121234567", "Test@Example.com", DateTime.UtcNow, isAvailable: true);

        var response = product.Adapt<CreateProductResponse>(MappingConfig.Config);

        Assert.Equal(product.Id, response.Id);
        Assert.Equal(product.Name.Value, response.Name);
        Assert.Equal(product.ManufacturePhone.Value, response.ManufacturePhone);
        Assert.Equal(product.ManufactureEmail.Value, response.ManufactureEmail);
        Assert.Equal(product.ProduceDate.Value, response.ProduceDate);
    }

    [Fact]
    public void Adapt_ProductToUpdateProductResponse_MapsAllValues()
    {
        var product = Product.Create("Galaxy S24", "09121234567", "Test@Example.com", DateTime.UtcNow, isAvailable: false);

        var response = product.Adapt<UpdateProductResponse>(MappingConfig.Config);

        Assert.Equal(product.Id, response.Id);
        Assert.Equal(product.Name.Value, response.Name);
        Assert.Equal(product.ManufacturePhone.Value, response.ManufacturePhone);
        Assert.Equal(product.ManufactureEmail.Value, response.ManufactureEmail);
        Assert.Equal(product.IsAvailable, response.IsAvailable);
    }

    [Fact]
    public void Adapt_ProductToProductDto_MapsAllValues()
    {
        var product = Product.Create("Galaxy S24", "09121234567", "Test@Example.com", DateTime.UtcNow, isAvailable: false);

        var dto = product.Adapt<ProductDto>(MappingConfig.Config);

        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.Name.Value, dto.Name);
        Assert.Equal(product.ManufacturePhone.Value, dto.ManufacturePhone);
        Assert.Equal(product.ManufactureEmail.Value, dto.ManufactureEmail);
        Assert.Equal(product.ProduceDate.Value, dto.ProduceDate);
        Assert.Equal(product.IsAvailable, dto.IsAvailable);
    }

    [Fact]
    public void Adapt_JwtResultToAuthResponse_MapsTokenValues()
    {
        var jwtResult = new JwtResult(
            Guid.NewGuid().ToString(),
            "access-token",
            "refresh-token",
            new DateTime(2026, 11, 24, 12, 0, 0, DateTimeKind.Utc));

        var response = jwtResult.Adapt<AuthResponse>(MappingConfig.Config);

        Assert.Equal(jwtResult.AccessToken, response.AccessToken);
        Assert.Equal(jwtResult.RefreshToken, response.RefreshToken);
        Assert.Equal(jwtResult.ExpirationDate, response.ExpirationDate);
    }
}
