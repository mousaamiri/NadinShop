using Mapster;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.Mapping;
using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Features.Products.Dtos;

namespace NadinShop.Application.Features.Products.Queries.Get;

public class GetProductsQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    public async Task<Result<IReadOnlyList<ProductDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await productRepository.ListAsync(cancellationToken);

        return products.Adapt<List<ProductDto>>(MappingConfig.Config);
    }
}
