using CashFlow.Ledger.Domain.Repositories;
using CashFlow.Ledger.Infrastructure.Configuration;
using CashFlow.Ledger.Infrastructure.Persistence;
using CashFlow.Ledger.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Ledger.Infrastructure.DependencyInjection;

public static class LedgerInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddLedgerPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var persistenceOptions = configuration
            .GetSection(LedgerPersistenceOptions.SectionName)
            .Get<LedgerPersistenceOptions>() ?? new LedgerPersistenceOptions();

        if (string.Equals(persistenceOptions.Mode, "Initialization", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<LedgerDbContext>(options =>
                options.UseInMemoryDatabase(persistenceOptions.InMemoryDatabaseName));
        }
        else if (string.Equals(persistenceOptions.Mode, "SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = configuration.GetConnectionString("LedgerDatabase");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("A connection string LedgerDatabase e obrigatoria para o modo SqlServer.");
            }

            // The SQL Server provider is compatible with Azure SQL Database.
            services.AddDbContext<LedgerDbContext>(options => options.UseSqlServer(connectionString));
        }
        else
        {
            throw new InvalidOperationException($"Modo de persistencia invalido: {persistenceOptions.Mode}.");
        }

        services.AddScoped<ILedgerEntryRepository, EfLedgerEntryRepository>();
        services.AddScoped<IIdempotencyRecordRepository, EfIdempotencyRecordRepository>();
        services.AddScoped<IIntegrationMessageRepository, EfIntegrationMessageRepository>();
        services.AddScoped<ILedgerUnitOfWork, EfLedgerUnitOfWork>();

        return services;
    }
}
