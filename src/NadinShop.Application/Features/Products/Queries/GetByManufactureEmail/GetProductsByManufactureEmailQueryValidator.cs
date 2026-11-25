using FluentValidation;
using NadinShop.Domain.Common;
using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Application.Features.Products.Queries.GetByManufactureEmail;

public sealed class GetProductsByManufactureEmailQueryValidator
    : AbstractValidator<GetProductsByManufactureEmailQuery>
{
    public GetProductsByManufactureEmailQueryValidator()
    {
        RuleFor(query => query.ManufactureEmail)
            .NotEmpty()
            .WithMessage("Manufacturer email is required.")
            .Must(BeValidManufactureEmail)
            .WithMessage("Manufacturer email format is invalid.");
    }

    private static bool BeValidManufactureEmail(string email)
    {
        try
        {
            ManufactureEmail.Create(email);
            return true;
        }
        catch (DomainException)
        {
            return false;
        }
    }
}
