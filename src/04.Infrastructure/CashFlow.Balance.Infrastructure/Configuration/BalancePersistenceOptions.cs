namespace CashFlow.Balance.Infrastructure.Configuration;

public sealed class BalancePersistenceOptions
{
    public const string SectionName = "BalancePersistence";

    public string Mode { get; set; } = "Initialization";

    public string InMemoryDatabaseName { get; set; } = "CashFlowBalance";
}
