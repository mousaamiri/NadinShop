using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this ControllerBase controller, Result result)
        => result.IsSuccess ? controller.NoContent() : controller.ToProblem(result.Error);

    public static ActionResult<TValue> ToActionResult<TValue>(
        this ControllerBase controller, Result<TValue> result)
        => result.IsSuccess ? controller.Ok(result.Value) : controller.ToProblem(result.Error);

    public static ObjectResult ToProblem(this ControllerBase controller, Error error)
    {
        int statusCode = ToStatusCode(error.ErrorType);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = ToTitle(error.ErrorType),
            Detail = error.Message,
            Instance = controller.HttpContext.Request.Path,
            Extensions =
            {
                ["code"] = error.Code,
                ["traceId"] = Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier
            }
        };

        return new ObjectResult(problem)
        {
            StatusCode = statusCode,
            ContentTypes = { "application/problem+json" }
        };
    }

    private static int ToStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden or ErrorType.Verification => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string ToTitle(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => "Validation failed",
        ErrorType.NotFound => "Resource not found",
        ErrorType.Conflict => "Conflict",
        ErrorType.Unauthorized => "Unauthorized",
        ErrorType.Forbidden => "Forbidden",
        ErrorType.Verification => "Verification required",
        _ => "Request failed"
    };
}
