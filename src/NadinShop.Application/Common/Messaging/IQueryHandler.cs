using MediatR;
using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Application.Common.Messaging;

public interface IQueryHandler<in TQuery, TResponse>: IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
