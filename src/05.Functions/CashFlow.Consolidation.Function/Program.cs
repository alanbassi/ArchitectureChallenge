using CashFlow.Balance.Application.DailyBalances.Commands.ProcessLedgerEntryRegistered;
using CashFlow.Balance.Domain.Services;
using CashFlow.Balance.Infrastructure.DependencyInjection;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddBalancePersistence(context.Configuration);
        services.AddScoped<DailyBalanceService>();
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.Assemblies = [typeof(ProcessLedgerEntryRegisteredCommand)];
        });
    })
    .Build();

host.Run();
