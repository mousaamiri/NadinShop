using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace NadinShop.Api.Extensions;

public static class ProblemDetailsResponses
{
    public const string UnauthenticatedCode = "Error.Unauthenticated";
    public const string ForbiddenCode = "Error.Forbidden";

    public static ProblemDetails Create(HttpContext context, int statusCode,
        string title, string detail, string code)
        => new()
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Extensions =
            {
                ["code"] = code,
                ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier
            }
        };

    public static async Task WriteAsync(HttpContext context, int statusCode,
        string title, string detail, string code)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(
            Create(context, statusCode, title, detail, code),
            options: null,
            contentType: "application/problem+json");
    }


}
