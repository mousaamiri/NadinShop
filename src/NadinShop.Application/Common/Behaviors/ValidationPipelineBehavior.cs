using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation;
using MediatR;
using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Application.Common.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private static readonly ConcurrentDictionary<Type, Func<Error[], Result>> _validationResultFactories = new();
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var errors = validationResults.Where(r => !r.IsValid).SelectMany(r => r.Errors)
            .Select(failure => Error.Validation(code: $"{typeof(TRequest).Name}.{failure.PropertyName}", message: failure.ErrorMessage))
            .DistinctBy(e => e.Code)
            .ToArray();

        if (errors.Length > 0)
            return (TResponse)CreateValidationResult(errors);

        return await next(cancellationToken);
    }

    private static Result CreateValidationResult(Error[] errors)
    {
        Func<Error[], Result> factory = _validationResultFactories.GetOrAdd(typeof(TResponse), static responseType =>
        {
            if (responseType == typeof(Result))
                return Result.WithErrors;

            //Generic Result<T> type
            Type resultType = responseType.GetGenericArguments()[0];
            Type resultGenericType = typeof(Result<>).MakeGenericType(resultType);
            MethodInfo method = resultGenericType.GetMethod(nameof(Result<object>.WithErrors), BindingFlags.Public | BindingFlags.Static)!;
            return errors => (Result)method.Invoke(null, [errors])!;
        });
        return factory(errors);
    }
}
