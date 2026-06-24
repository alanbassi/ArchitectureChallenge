using CashFlow.Balance.Application.DailyBalances.Queries.Get;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Balance.Infrastructure.DependencyInjection;

public static class BalanceServiceCollectionExtensions
{
    public static IServiceCollection AddBalance(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddBalanceApplication();
        services.AddBalancePersistence(configuration);

        return services;
    }

    private static void AddBalanceApplication(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.Assemblies = [typeof(GetDailyBalanceQuery)];
        });
    }
}
