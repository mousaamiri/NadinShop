using System.Text.RegularExpressions;
using NadinShop.Domain.Common;

namespace NadinShop.Domain.Entities.Products.ValueObjects;

public sealed class ManufacturePhone : ValueObject<string>
{
    public const int MinLength = 7;
    public const int MaxLength = 15;
    public const int MaxStoredLength = 32;

    // Allowed: an optional "+" at the beginning, digits, and separators such as spaces, hyphens, and parentheses
    private static readonly Regex PhoneRegex =
    new (@"^\+?[0-9][0-9\s\-()]*$", RegexOptions.Compiled);

    private ManufacturePhone(string value) : base(value)
    {
    }

    public static ManufacturePhone Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ManufacturePhoneCannotBeEmptyException();

        var trimmed = value.Trim();

        if (trimmed.Length > MaxStoredLength)
            throw new ManufacturePhoneIsTooLongException(MaxStoredLength);

        // We check the length without separators
        var digitsOnly = new string(trimmed.Where(char.IsDigit).ToArray());
        if (digitsOnly.Length < MinLength || digitsOnly.Length > MaxLength)
            throw new ManufacturePhoneLengthIsInvalidException(MinLength, MaxLength);

        if (!PhoneRegex.IsMatch(trimmed))
            throw new ManufacturePhoneFormatIsInvalidException();

        return new ManufacturePhone(trimmed);
    }

    public override string ToString() => Value;

    public static implicit operator string(ManufacturePhone phone) => phone.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

public sealed class ManufacturePhoneCannotBeEmptyException()
    : DomainException("The manufacturer's phone number cannot be empty.");

public sealed class ManufacturePhoneLengthIsInvalidException(
    int minLength,
    int maxLength)
    : DomainException(
        $"The manufacturer's phone number must be between {minLength} and {maxLength} digits long.");

public sealed class ManufacturePhoneFormatIsInvalidException()
    : DomainException("The manufacturer's phone number format is invalid.");

public sealed class ManufacturePhoneIsTooLongException(int maxLength)
    : DomainException(
        $"The manufacturer's phone number must not exceed {maxLength} characters.");
