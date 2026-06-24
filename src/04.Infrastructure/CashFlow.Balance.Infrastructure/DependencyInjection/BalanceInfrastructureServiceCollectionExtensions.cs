using CashFlow.Balance.Domain.Repositories;
using CashFlow.Balance.Infrastructure.Configuration;
using CashFlow.Balance.Infrastructure.Persistence;
using CashFlow.Balance.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Balance.Infrastructure.DependencyInjection;

public static class BalanceInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddBalancePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration
            .GetSection(BalancePersistenceOptions.SectionName)
            .Get<BalancePersistenceOptions>() ?? new BalancePersistenceOptions();

        if (string.Equals(options.Mode, "Initialization", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<BalanceDbContext>(builder =>
                builder.UseInMemoryDatabase(options.InMemoryDatabaseName));
        }
        else if (string.Equals(options.Mode, "SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = configuration.GetConnectionString("BalanceDatabase");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("A BalanceDatabase connection string is required for SqlServer mode.");
            }

            services.AddDbContext<BalanceDbContext>(builder => builder.UseSqlServer(connectionString));
        }
        else
        {
            throw new InvalidOperationException($"Invalid balance persistence mode: {options.Mode}.");
        }

        services.AddScoped<IDailyBalanceRepository, EfDailyBalanceRepository>();
        services.AddScoped<IInboxMessageRepository, EfInboxMessageRepository>();
        services.AddScoped<IBalanceUnitOfWork, EfBalanceUnitOfWork>();

        return services;
    }
}
