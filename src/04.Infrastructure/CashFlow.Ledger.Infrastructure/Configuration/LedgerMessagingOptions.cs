namespace CashFlow.Ledger.Infrastructure.Configuration;

public sealed class LedgerMessagingOptions
{
    public const string SectionName = "Messaging";

    public string Mode { get; set; } = "Disabled";

    public string ConnectionString { get; set; } = string.Empty;

    public string EntityName { get; set; } = "ledger-entry-registered";

    public int BatchSize { get; set; } = 50;

    public int PollingIntervalSeconds { get; set; } = 10;
}
