using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NadinShop.Api.Extensions;
using NadinShop.Domain.Common;

namespace NadinShop.Api.Middleware;

public sealed partial class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private const string DomainErrorCode = "Error.Domain";
    private const string UniqueConstraintErrorCode = "Error.UniqueConstraintViolation";
    private const string UnexpectedErrorCode = "Error.InternalServerError";

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
            await WriteAuthenticationProblemAsync(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            LogRequestCancelled(logger, context.Request.Method, context.Request.Path);
        }
        catch (DomainException exception)
        {
            LogDomainViolation(logger, exception, context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            await WriteProblemAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Domain rule violation",
                exception.Message,
                DomainErrorCode);
        }
        catch (DbUpdateException exception)
        {
            LogUniqueConstraintViolation(logger, exception, context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            await WriteProblemAsync(
                context,
                StatusCodes.Status409Conflict,
                "Conflict",
                "The request conflicts with an existing record.",
                UniqueConstraintErrorCode);
        }
        catch (Exception exception)
        {
            LogUnhandledException(logger, exception, context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                "An unexpected error occurred while processing the request.",
                UnexpectedErrorCode);
        }
    }

    private static async Task WriteAuthenticationProblemAsync(HttpContext context)
    {
        if (context.Response.HasStarted)
            return;

        switch (context.Response.StatusCode)
        {
            case StatusCodes.Status401Unauthorized:
                await ProblemDetailsResponses.WriteAsync(
                    context,
                    StatusCodes.Status401Unauthorized,
                    "Authentication required",
                    "Authentication failed: the request has no valid access token. " +
                    "Sign in again and send the token in the Authorization header as 'Bearer {token}'.",
                    ProblemDetailsResponses.UnauthenticatedCode);
                break;
            case StatusCodes.Status403Forbidden:
                await ProblemDetailsResponses.WriteAsync(
                    context,
                    StatusCodes.Status403Forbidden,
                    "Access denied",
                    "You are authenticated but do not have permission to perform this action.",
                    ProblemDetailsResponses.ForbiddenCode);
                break;
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int statusCode,
        string title, string detail, string code)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
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

        await context.Response.WriteAsJsonAsync(problem,
            options: null, contentType: "application/problem+json");
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "Request {Method} {Path} was cancelled by the client.")]
    private static partial void LogRequestCancelled(ILogger logger, string method, string path);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning,
        Message = "Domain rule violation while processing {Method} {Path}.")]
    private static partial void LogDomainViolation(ILogger logger, Exception exception,
        string method, string path);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error,
        Message = "Unhandled exception while processing {Method} {Path}.")]
    private static partial void LogUnhandledException(ILogger logger, Exception exception,
        string method, string path);

    [LoggerMessage(EventId = 4, Level = LogLevel.Warning,
        Message = "Unique constraint violation while processing {Method} {Path}.")]
    private static partial void LogUniqueConstraintViolation(ILogger logger, Exception exception,
        string method, string path);
}
