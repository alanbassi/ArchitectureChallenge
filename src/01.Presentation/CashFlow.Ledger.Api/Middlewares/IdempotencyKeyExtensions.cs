namespace CashFlow.Ledger.Api.Middlewares;

public static class IdempotencyKeyExtensions
{
    public static string GetIdempotencyKey(this HttpContext context)
        => (string)context.Items["Idempotency-Key"]!;
}
