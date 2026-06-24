using CashFlow.Ledger.Api.Requests;
using CashFlow.Ledger.Api.Responses;
using CashFlow.Ledger.Application.LedgerEntries.Commands.Register;
using CashFlow.Ledger.Domain.Entities;
using CashFlow.Ledger.Domain.Enums;

namespace CashFlow.Ledger.Api.Mappings;

public static class LedgerEntryMappings
{
    public static RegisterLedgerEntryCommand ToCommand(
        this CreateLedgerEntryRequest request,
        string idempotencyKey) =>
        new(
            request.MerchantId,
            request.Type.ToEntryType(),
            request.Amount,
            request.BusinessDate,
            request.Description,
            idempotencyKey);

    public static LedgerEntryResponse ToResponse(this RegisterLedgerEntryResult result) =>
        new(
            result.Id,
            result.EntryType.ToApiValue(),
            result.Amount,
            result.BusinessDate,
            result.Description,
            result.RegisteredAtUtc);

    public static LedgerEntryListItemResponse ToListItemResponse(this LedgerEntry ledgerEntry) =>
        new(
            ledgerEntry.Id,
            ledgerEntry.EntryType.ToApiValue(),
            ledgerEntry.Amount.Amount,
            ledgerEntry.BusinessDate,
            ledgerEntry.Description);

    private static EntryType ToEntryType(this string type) =>
        type.Trim().ToLowerInvariant() switch
        {
            "credito" => EntryType.Credit,
            "debito" => EntryType.Debit,
            _ => throw new ArgumentOutOfRangeException(nameof(type), "O tipo deve ser credito ou debito.")
        };

    private static string ToApiValue(this EntryType entryType) =>
        entryType switch
        {
            EntryType.Credit => "credito",
            EntryType.Debit => "debito",
            _ => throw new ArgumentOutOfRangeException(nameof(entryType), entryType, "Tipo de lancamento invalido.")
        };
}
