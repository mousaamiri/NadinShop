using NadinShop.Application.Common.Messaging;

namespace NadinShop.Application.Features.Products.Commands.Delete;

public record DeleteProductCommand(Guid Id) : ICommand;
