using NadinShop.Application.Common.Messaging;

namespace NadinShop.Application.Features.Products.Commands.Update;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    bool IsAvailable) : ICommand<UpdateProductResponse>;
