using MediatR;
using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Application.Common.Messaging;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;
