using System.Net;
using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Application.Common.Messaging;

public interface ICommand
{
}

public interface ICommand<TResponse> : ICommand
{
}

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface IQuery<TResponse>
{
}

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken = default);
}
