using System.Text.RegularExpressions;
using FluentValidation;
using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Application.Features.Products.Common;

internal static class ProductNameValidationExtensions
{
    private static readonly Regex ProductNameRegex = new(
        @"^[a-zA-Z](?:[a-zA-Z0-9\u200C]+|\s+|\p{P}(?![\p{P}]))*$",
        RegexOptions.Compiled);

    public static IRuleBuilderOptions<T, string> ValidProductName<T>(
        this IRuleBuilderInitial<T, string> ruleBuilder)
        => ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .Must(HasValidLength)
            .WithMessage($"Product name length must be between {ProductName.MinLength} and {ProductName.MaxLength} characters.")
            .Must(HasValidFormat)
            .WithMessage("Product name format is invalid.");

    private static bool HasValidLength(string? name)
    {
        if (name is null)
            return false;

        var length = name.Trim().Length;
        return length >= ProductName.MinLength && length <= ProductName.MaxLength;
    }

    private static bool HasValidFormat(string? name)
        => name is not null && ProductNameRegex.IsMatch(name.Trim());
}
