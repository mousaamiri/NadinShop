using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Products.Common;

namespace NadinShop.Application.Features.Products.Commands.Delete;

public class DeleteProductCommandHandler(
    IProductRepository productRepository,
    ICurrentUserService currentUserService)
    : ICommandHandler<DeleteProductCommand>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser is null)
            return Error.Unauthorized("Error.Unauthenticated", "User is not authenticated.");

        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Error.NotFound("Error.ProductNotFound", "Product was not found.");

        if (!ProductOwnership.IsOwnedBy(product, currentUser))
            return Error.Forbidden("Error.ProductAccessDenied", "You are not allowed to delete this product.");

        productRepository.Remove(product);

        return Result.Success();
    }
}
