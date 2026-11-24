using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Application.Features.Products.Commands.Create;

public class CreateProductCommandHandler: ICommandHandler<CreateProductCommand, CreateProductResponse>
{
    public Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
