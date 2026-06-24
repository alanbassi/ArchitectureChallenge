using CashFlow.Ledger.Application.Behaviors;
using CashFlow.Ledger.Application.LedgerEntries.Commands.Register;
using CashFlow.Ledger.Domain.Services;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Ledger.Infrastructure.DependencyInjection;

public static class LedgerServiceCollectionExtensions
{
    public static IServiceCollection AddLedger(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddLedgerDomainServices();
        services.AddLedgerApplication();
        services.AddLedgerPersistence(configuration);
        services.AddLedgerMessaging(configuration);

        return services;
    }

    private static void AddLedgerDomainServices(this IServiceCollection services)
    {
        services.AddScoped<LedgerEntryService>();
        services.AddScoped<IdempotencyService>();
    }

    private static void AddLedgerApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidator<RegisterLedgerEntryCommand>, RegisterLedgerEntryCommandValidator>();
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.Assemblies = [typeof(RegisterLedgerEntryCommand)];
            options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
        });
    }
}
