using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CashFlow.Ledger.Domain.Enums;

namespace CashFlow.Ledger.Domain.Services;

public sealed class IdempotencyService
{
    public string CreateRequestFingerprint(
        string merchantId,
        EntryType entryType,
        decimal amount,
        DateOnly businessDate,
        string? description)
    {
        var content = string.Join(
            "\n",
            Normalize(merchantId),
            ((int)entryType).ToString(CultureInfo.InvariantCulture),
            decimal.Round(amount, 2, MidpointRounding.ToEven).ToString("0.00", CultureInfo.InvariantCulture),
            businessDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Normalize(description));

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)));
    }

    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;
}
