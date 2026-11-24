using FluentValidation;
using NadinShop.Application.Features.Products.Common;

namespace NadinShop.Application.Features.Products.Commands.Update;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Product id is required.");

        RuleFor(command => command.Name).ValidProductName();
    }
}
