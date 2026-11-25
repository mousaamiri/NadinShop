using NadinShop.Application.Features.Products.Queries.GetById;

namespace NadinShop.Application.Tests.Features.Products.Queries.GetById;

public class GetProductByIdQueryValidatorTests
{
    private readonly GetProductByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidId_ReturnsValid()
    {
        var result = _validator.Validate(new GetProductByIdQuery(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyId_ReturnsIdError()
    {
        var result = _validator.Validate(new GetProductByIdQuery(Guid.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(GetProductByIdQuery.Id));
    }
}
