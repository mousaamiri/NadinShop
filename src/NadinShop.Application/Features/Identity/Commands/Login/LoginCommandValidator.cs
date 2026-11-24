using FluentValidation;

namespace NadinShop.Application.Features.Identity.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email is required.")
            .MaximumLength(254)
            .WithMessage("Email must not exceed 254 characters.")
            .EmailAddress()
            .WithMessage("Email format is invalid.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}
