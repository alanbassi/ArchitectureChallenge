using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Ledger.Api.Middlewares;

public sealed class IdempotencyKeyMiddleware(RequestDelegate next)
{
    private const string HeaderName = "Idempotency-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
        {
            await next(context);
            return;
        }

        var header = context.Request.Headers[HeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(header))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";

            var problem = new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [HeaderName] = ["The Idempotency-Key header is required."]
            })
            {
                Status = StatusCodes.Status400BadRequest
            };

            await context.Response.WriteAsJsonAsync(problem);
            return;
        }

        context.Items[HeaderName] = header;
        await next(context);
    }
}
