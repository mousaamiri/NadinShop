using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NadinShop.Api.Extensions;

public static class InvalidModelStateResponseFactory
{
    public const string InvalidJsonCode = "Error.InvalidJson";
    public const string ValidationCode = "Error.Validation";

    public static IActionResult Create(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (ContainsMalformedJson(context.ModelState))
        {
            return BadRequestProblem(
                context.HttpContext,
                "Invalid request body",
                "The request body is not valid JSON. Check for unescaped characters inside string values: " +
                "a line break must be written as \\n, a double quote as \\\" and a backslash as \\\\. " +
                "The body must be a single well-formed JSON object.",
                InvalidJsonCode);
        }

        if (IsMissingRequestBody(context))
        {
            return BadRequestProblem(
                context.HttpContext,
                "Invalid request body",
                "A JSON request body is required. Send a well-formed JSON object in the request body.",
                InvalidJsonCode);
        }

        ProblemDetails problem = ProblemDetailsResponses.Create(
            context.HttpContext,
            StatusCodes.Status400BadRequest,
            "Validation failed",
            "One or more fields in the request are invalid. See the errors collection for details.",
            ValidationCode);

        problem.Extensions["errors"] = ToErrorDictionary(context.ModelState);

        return new ObjectResult(problem)
        {
            StatusCode = StatusCodes.Status400BadRequest,
            ContentTypes = { "application/problem+json" }
        };
    }

    private static ObjectResult BadRequestProblem(HttpContext httpContext,
        string title, string detail, string code)
        => new(ProblemDetailsResponses.Create(
            httpContext, StatusCodes.Status400BadRequest, title, detail, code))
        {
            StatusCode = StatusCodes.Status400BadRequest,
            ContentTypes = { "application/problem+json" }
        };

    private static bool ContainsMalformedJson(ModelStateDictionary modelState)
        => modelState.Values
            .SelectMany(entry => entry.Errors)
            .Any(error => error.Exception is JsonException
                          || error.Exception?.InnerException is JsonException
                          || error.ErrorMessage.Contains("JSON", StringComparison.OrdinalIgnoreCase));

    private static bool IsMissingRequestBody(ActionContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor action)
            return false;

        var bodyParameters = action.Parameters
            .Where(parameter => parameter.BindingInfo?.BindingSource == BindingSource.Body)
            .Select(parameter => parameter.Name)
            .Where(name => name is not null)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (bodyParameters.Count == 0)
            return false;

        var entries = context.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .ToArray();

        return entries.Length > 0
               && entries.All(entry => (string.IsNullOrEmpty(entry.Key) || bodyParameters.Contains(entry.Key))
                                       && entry.Value!.Errors.Any(error =>
                                           error.ErrorMessage.Contains("required", StringComparison.OrdinalIgnoreCase)));
    }

    private static Dictionary<string, string[]> ToErrorDictionary(ModelStateDictionary modelState)
        => modelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .GroupBy(entry => string.IsNullOrEmpty(entry.Key) ? "request" : entry.Key)
            .ToDictionary(
                group => group.Key,
                group => group.SelectMany(entry => entry.Value!.Errors)
                    .Select(error => error.ErrorMessage)
                    .Distinct(StringComparer.Ordinal)
                    .ToArray());
}
