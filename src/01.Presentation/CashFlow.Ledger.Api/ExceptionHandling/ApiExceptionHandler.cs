using CashFlow.Ledger.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Ledger.Api.ExceptionHandling;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case ValidationException validationException:
                await WriteValidationProblemAsync(httpContext, validationException, cancellationToken);
                return true;

            case IdempotencyConflictException:
                await WriteProblemAsync(
                    httpContext,
                    StatusCodes.Status409Conflict,
                    "Idempotency conflict",
                    cancellationToken);
                return true;

            case ArgumentException argumentException:
                await WriteProblemAsync(
                    httpContext,
                    StatusCodes.Status400BadRequest,
                    argumentException.Message,
                    cancellationToken);
                return true;

            default:
                logger.LogError(exception, "An unexpected error occurred while processing the request.");
                await WriteProblemAsync(
                    httpContext,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.",
                    cancellationToken);
                return true;
        }
    }

    private static Task WriteValidationProblemAsync(
        HttpContext httpContext,
        ValidationException exception,
        CancellationToken cancellationToken)
    {
        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());
        var problem = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
    }

    private static Task WriteProblemAsync(
        HttpContext httpContext,
        int statusCode,
        string title,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = statusCode;

        return httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title
            },
            cancellationToken);
    }
}
