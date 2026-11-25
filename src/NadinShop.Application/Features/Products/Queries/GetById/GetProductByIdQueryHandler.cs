using Mapster;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.Mapping;
using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Features.Products.Dtos;

namespace NadinShop.Application.Features.Products.Queries.GetById;

public class GetProductByIdQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Error.NotFound("Error.ProductNotFound", "Product was not found.");

        return product.Adapt<ProductDto>(MappingConfig.Config);
    }
}
