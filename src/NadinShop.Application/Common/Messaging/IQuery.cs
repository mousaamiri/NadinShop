using MediatR;
using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Application.Common.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
