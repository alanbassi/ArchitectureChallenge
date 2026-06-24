namespace CashFlow.Ledger.Infrastructure.Configuration;

public sealed class LedgerPersistenceOptions
{
    public const string SectionName = "Persistence";

    public string Mode { get; set; } = "Initialization";

    public string InMemoryDatabaseName { get; set; } = "CashFlow";
}
