namespace CashFlow.SharedKernel.Guards;

public static class Guard
{
    public static void NotEmpty(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} is required.", paramName);
    }

    public static void NotEmpty(Guid value, string paramName)
    {
        if (value == Guid.Empty)
            throw new ArgumentException($"{paramName} is required.", paramName);
    }

    public static void MaxLength(string? value, int maxLength, string paramName)
    {
        if (value?.Trim().Length > maxLength)
            throw new ArgumentOutOfRangeException(paramName, $"{paramName} must have at most {maxLength} characters.");
    }

    public static void DefinedEnum<T>(T value, string paramName) where T : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentOutOfRangeException(paramName, $"Invalid value for {paramName}.");
    }

    public static void NotDefault<T>(T value, string paramName) where T : struct, IEquatable<T>
    {
        if (value.Equals(default))
            throw new ArgumentException($"{paramName} is required.", paramName);
    }

    public static void MustBeUtc(DateTimeOffset value, string paramName)
    {
        if (value.Offset != TimeSpan.Zero)
            throw new ArgumentException($"{paramName} must be in UTC.", paramName);
    }
}
