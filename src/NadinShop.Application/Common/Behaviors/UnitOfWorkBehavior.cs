using MediatR;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Application.Common.Behaviors;

public class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (IsNotCommand())
            return await next(cancellationToken);

        var response = await next(cancellationToken);
        if (response.IsSuccess)
        {
            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                //TODO: implement logging .... 
                Console.WriteLine(ex);
                throw;
            }
        }
        return response;
    }
    //Check if the request is a command and if the response is a Result type
    private static bool IsNotCommand()
        => !typeof(TRequest).Name.EndsWith("Command");
}
