namespace CashFlow.Ledger.Domain.Exceptions;

public sealed class IdempotencyConflictException : Exception
{
    public IdempotencyConflictException()
        : base("A chave de idempotencia ja foi utilizada com outro conteudo.")
    {
    }
}
