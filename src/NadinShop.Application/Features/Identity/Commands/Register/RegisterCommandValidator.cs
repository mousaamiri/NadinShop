using System.Text.RegularExpressions;
using FluentValidation;
using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Application.Features.Identity.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private static readonly Regex PhoneRegex = new(
        @"^\+?[0-9][0-9\s\-()]*$",
        RegexOptions.Compiled);

    public RegisterCommandValidator()
    {
        RuleFor(command => command.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email is required.")
            .MaximumLength(254)
            .WithMessage("Email must not exceed 254 characters.")
            .EmailAddress()
            .WithMessage("Email format is invalid.");

        RuleFor(command => command.Phone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .MaximumLength(ManufacturePhone.MaxStoredLength)
            .WithMessage($"Phone number must not exceed {ManufacturePhone.MaxStoredLength} characters.")
            .Must(BeValidPhone)
            .WithMessage($"Phone number must contain between {ManufacturePhone.MinLength} and {ManufacturePhone.MaxLength} digits and use a valid format.");

        RuleFor(command => command.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(128)
            .WithMessage("Password must not exceed 128 characters.");
    }

    private static bool BeValidPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        var trimmed = phone.Trim();
        var digitCount = trimmed.Count(char.IsDigit);

        return digitCount >= ManufacturePhone.MinLength
               && digitCount <= ManufacturePhone.MaxLength
               && PhoneRegex.IsMatch(trimmed);
    }
}
