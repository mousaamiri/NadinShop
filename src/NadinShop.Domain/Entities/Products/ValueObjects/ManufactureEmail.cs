using System.Text.RegularExpressions;
using NadinShop.Domain.Common;

namespace NadinShop.Domain.Entities.Products.ValueObjects;

public sealed class ManufactureEmail : ValueObject<string>
{
    public const int MinLength = 5;

    public const int MaxLength = 254;

    private const int MaxLocalPartLength = 64;

    private static readonly Regex EmailRegex = new(
        @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*" +
        @"@(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.)+[a-z]{2,63}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private ManufactureEmail(string value) : base(value)
    {
    }

    public static ManufactureEmail Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ManufactureEmailCannotBeEmptyException();

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length < MinLength || normalized.Length > MaxLength)
            throw new ManufactureEmailLengthIsInvalidException(MinLength, MaxLength);

        if (!EmailRegex.IsMatch(normalized))
            throw new ManufactureEmailFormatIsInvalidException();

        var localPart = normalized[..normalized.LastIndexOf('@')];
        if (localPart.Length > MaxLocalPartLength)
            throw new ManufactureEmailFormatIsInvalidException();

        return new ManufactureEmail(normalized);
    }

    // Local part: allowed atoms separated by single dots (no leading, trailing, or consecutive dots)
    // Domain: labels separated by dots, no leading/trailing hyphens

    public override string ToString() => Value;

    public static implicit operator string(ManufactureEmail email) => email.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

public sealed class ManufactureEmailCannotBeEmptyException()
    : DomainException("The manufacturer's email cannot be empty.");

public sealed class ManufactureEmailLengthIsInvalidException(int minLength, int maxLength)
    : DomainException(
        $"The manufacturer's email must be between {minLength} and {maxLength} characters long.");

public sealed class ManufactureEmailFormatIsInvalidException()
    : DomainException("The manufacturer's email format is invalid.");
