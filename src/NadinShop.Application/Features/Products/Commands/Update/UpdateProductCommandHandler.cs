using Mapster;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.Mapping;
using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Products.Common;

namespace NadinShop.Application.Features.Products.Commands.Update;

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICurrentUserService currentUserService)
    : ICommandHandler<UpdateProductCommand, UpdateProductResponse>
{
    public async Task<Result<UpdateProductResponse>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser is null)
            return Error.Unauthorized("Error.Unauthenticated", "User is not authenticated.");

        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Error.NotFound("Error.ProductNotFound", "Product was not found.");

        if (!ProductOwnership.IsOwnedBy(product, currentUser))
            return Error.Forbidden("Error.ProductAccessDenied", "You are not allowed to modify this product.");

        var productWithSameName = await productRepository.GetByNameAsync(request.Name, cancellationToken);
        if (productWithSameName is not null && productWithSameName.Id != product.Id)
            return Error.Conflict("Error.ProductNameAlreadyExists", "A product with the same name already exists.");

        //We just update name of product . 
        product.Rename(request.Name);

        if (request.IsAvailable)
            product.MarkAsAvailable();
        else
            product.MarkAsUnavailable();

        productRepository.Update(product);

        return product.Adapt<UpdateProductResponse>(MappingConfig.Config);
    }
}
