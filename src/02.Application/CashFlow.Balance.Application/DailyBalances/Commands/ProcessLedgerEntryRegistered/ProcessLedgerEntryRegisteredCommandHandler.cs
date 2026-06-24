using CashFlow.Balance.Domain.Entities;
using CashFlow.Balance.Domain.Repositories;
using CashFlow.Balance.Domain.Services;
using Mediator;

namespace CashFlow.Balance.Application.DailyBalances.Commands.ProcessLedgerEntryRegistered;

public sealed class ProcessLedgerEntryRegisteredCommandHandler(
    IDailyBalanceRepository dailyBalanceRepository,
    IInboxMessageRepository inboxMessageRepository,
    IBalanceUnitOfWork unitOfWork,
    DailyBalanceService dailyBalanceService)
    : ICommandHandler<ProcessLedgerEntryRegisteredCommand, ProcessLedgerEntryRegisteredResult>
{
    public async ValueTask<ProcessLedgerEntryRegisteredResult> Handle(
        ProcessLedgerEntryRegisteredCommand command,
        CancellationToken cancellationToken)
    {
        var existingInboxMessage = await inboxMessageRepository.GetByEventIdAsync(
            command.EventId,
            cancellationToken);

        if (existingInboxMessage is not null)
        {
            return new ProcessLedgerEntryRegisteredResult(true);
        }

        var dailyBalance = await dailyBalanceRepository.GetByMerchantAndBusinessDateAsync(
            command.MerchantId,
            command.BusinessDate,
            cancellationToken);

        if (dailyBalance is null)
        {
            dailyBalance = dailyBalanceService.Create(
                command.MerchantId,
                command.BusinessDate,
                command.OccurredAtUtc);
            await dailyBalanceRepository.AddAsync(dailyBalance, cancellationToken);
        }

        dailyBalanceService.Apply(
            dailyBalance,
            command.EntryType,
            command.Amount,
            command.OccurredAtUtc);

        await inboxMessageRepository.AddAsync(
            InboxMessage.Create(command.EventId, DateTimeOffset.UtcNow),
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProcessLedgerEntryRegisteredResult(false);
    }
}
