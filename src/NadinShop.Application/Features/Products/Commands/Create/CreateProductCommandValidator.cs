using FluentValidation;
using NadinShop.Application.Features.Products.Common;

namespace NadinShop.Application.Features.Products.Commands.Create;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Name).ValidProductName();

        RuleFor(command => command.ProduceDate)
            .Must(produceDate => produceDate != default)
            .WithMessage("Produce date is required.")
            .Must(produceDate => produceDate <= DateTime.UtcNow)
            .WithMessage("Produce date cannot be in the future.");
    }
}
