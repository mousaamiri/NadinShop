using Mapster;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Identity.Commands.Dtos;
using NadinShop.Application.Features.Products.Commands.Create;
using NadinShop.Application.Features.Products.Commands.Update;
using NadinShop.Domain.Entities.Products;

namespace NadinShop.Application.Common.Mapping;

public sealed class MappingConfig : IRegister
{
    private static readonly Lazy<TypeAdapterConfig> ConfigFactory = new(CreateConfig);

    public static TypeAdapterConfig Config => ConfigFactory.Value;

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, CreateProductResponse>()
            .Map(destination => destination.Id, source => source.Id)
            .Map(destination => destination.Name, source => source.Name.Value)
            .Map(destination => destination.ManufacturePhone, source => source.ManufacturePhone.Value)
            .Map(destination => destination.ManufactureEmail, source => source.ManufactureEmail.Value)
            .Map(destination => destination.ProduceDate, source => source.ProduceDate.Value);

        config.NewConfig<Product, UpdateProductResponse>()
            .Map(destination => destination.Id, source => source.Id)
            .Map(destination => destination.Name, source => source.Name.Value)
            .Map(destination => destination.ManufacturePhone, source => source.ManufacturePhone.Value)
            .Map(destination => destination.ManufactureEmail, source => source.ManufactureEmail.Value)
            .Map(destination => destination.IsAvailable, source => source.IsAvailable);

        config.NewConfig<JwtResult, AuthResponse>()
            .Map(destination => destination.AccessToken, source => source.AccessToken)
            .Map(destination => destination.RefreshToken, source => source.RefreshToken)
            .Map(destination => destination.ExpirationDate, source => source.ExpirationDate);
    }

    private static TypeAdapterConfig CreateConfig()
    {
        var config = new TypeAdapterConfig();
        new MappingConfig().Register(config);
        return config;
    }
}
