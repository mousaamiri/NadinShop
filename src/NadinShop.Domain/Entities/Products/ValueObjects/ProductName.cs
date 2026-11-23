using System.Text.RegularExpressions;
using NadinShop.Domain.Common;

namespace NadinShop.Domain.Entities.Products.ValueObjects;

public class ProductName : ValueObject<string>
{
    //Fields
    public const short MinLength = 5;
    public const short MaxLength = 200;

    //Product name validation rules :
    // - must start with a latin letter
    // - may contain latin letters,digits , spaces and punctuations
    // - allows multiple consecutive spaces but not consecutive punctuation
    private static readonly Regex TitleRegex = new(
        @"^[a-zA-Z](?:[a-zA-Z0-9\u200C]+|\s+|\p{P}(?![\p{P}]))*$",
        RegexOptions.Compiled
    );

    //Constructor
    public ProductName(string value) : base(value)
    {

    }
    //Factory method
    public static ProductName Create(string value)
    {

        if (string.IsNullOrWhiteSpace(value))
            throw new ProductNameCannotBeEmptyException();

        string trimmedValue = value.Trim();

        if (trimmedValue.Length < MinLength || trimmedValue.Length > MaxLength)
            throw new ProductNameLengthIsInvalidException(MinLength, MaxLength);

        if (!TitleRegex.IsMatch(trimmedValue))
            throw new ProductNameFormatIsInvalidException();

        return new ProductName(trimmedValue);
    }
    //base method
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
    public static implicit operator string(ProductName value) => value;
}

public sealed class ProductNameCannotBeEmptyException()
    : DomainException("The product name cannot be empty.");

public sealed class ProductNameLengthIsInvalidException(short minLength, short maxLength)
    : DomainException(
        $"The product name length must be between {minLength} and {maxLength} characters.");

public sealed class ProductNameFormatIsInvalidException()
    : DomainException("The product name format is invalid.");
