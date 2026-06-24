using FluentValidation;

namespace CashFlow.Ledger.Application.LedgerEntries.Commands.Register;

public sealed class RegisterLedgerEntryCommandValidator : AbstractValidator<RegisterLedgerEntryCommand>
{
    public RegisterLedgerEntryCommandValidator()
    {
        RuleFor(command => command.MerchantId)
            .NotEmpty();

        RuleFor(command => command.EntryType)
            .IsInEnum();

        RuleFor(command => command.Amount)
            .GreaterThan(0);

        RuleFor(command => command.BusinessDate)
            .NotEqual(default(DateOnly));

        RuleFor(command => command.IdempotencyKey)
            .NotEmpty()
            .MaximumLength(128);
    }
}
