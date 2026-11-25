using Mapster;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.Mapping;
using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Products.Common;
using NadinShop.Domain.Entities.Products;

namespace NadinShop.Application.Features.Products.Commands.Create;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICurrentUserService currentUserService)
    : ICommandHandler<CreateProductCommand, CreateProductResponse>
{
    public async Task<Result<CreateProductResponse>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser is null)
            return Error.Unauthorized("Error.Unauthenticated", "User is not authenticated.");

        var existingProduct = await productRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingProduct is not null)
            return Error.Conflict("Error.ProductNameAlreadyExists", "A product with the same name already exists.");

        var product = Product.Create(
            request.Name,
            currentUser.Phone,
            currentUser.Email,
            request.ProduceDate,
            isAvailable: true);

        productRepository.Add(product);

        return product.Adapt<CreateProductResponse>(MappingConfig.Config);
    }
}
