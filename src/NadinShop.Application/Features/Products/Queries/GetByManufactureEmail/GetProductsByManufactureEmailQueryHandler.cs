using Mapster;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.Mapping;
using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Features.Products.Dtos;

namespace NadinShop.Application.Features.Products.Queries.GetByManufactureEmail;

public class GetProductsByManufactureEmailQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductsByManufactureEmailQuery, IReadOnlyList<ProductDto>>
{
    public async Task<Result<IReadOnlyList<ProductDto>>> Handle(
        GetProductsByManufactureEmailQuery request,
        CancellationToken cancellationToken)
    {
        var products = await productRepository.ListByManufactureEmailAsync(
            request.ManufactureEmail,
            cancellationToken);

        return products.Adapt<List<ProductDto>>(MappingConfig.Config);
    }
}
