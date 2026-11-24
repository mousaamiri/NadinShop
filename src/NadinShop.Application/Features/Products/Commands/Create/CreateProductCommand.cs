using NadinShop.Application.Common.Messaging;

namespace NadinShop.Application.Features.Products.Commands.Create;

public record CreateProductCommand(string Name,DateTime ProduceDate) :ICommand<CreateProductResponse>;
