using FluentValidation;

namespace NadinShop.Application.Features.Products.Queries.GetById;

public sealed class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty()
            .WithMessage("Product id is required.");
    }
}
