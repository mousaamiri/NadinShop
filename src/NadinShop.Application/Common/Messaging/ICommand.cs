using System.Net;
using MediatR;
using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Application.Common.Messaging;

public interface ICommand : IRequest<Result>;
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
