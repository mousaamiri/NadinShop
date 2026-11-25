using NadinShop.Application.Features.Products.Queries.GetByManufactureEmail;

namespace NadinShop.Application.Tests.Features.Products.Queries.GetByManufactureEmail;

public class GetProductsByManufactureEmailQueryValidatorTests
{
    private readonly GetProductsByManufactureEmailQueryValidator _validator = new();

    [Theory]
    [InlineData("Test@Example.com")]
    [InlineData("support@nadinshop.com")]
    public void Validate_WithValidEmail_ReturnsValid(string email)
    {
        var result = _validator.Validate(new GetProductsByManufactureEmailQuery(email));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    [InlineData("missing@domain")]
    public void Validate_WithInvalidEmail_ReturnsEmailError(string email)
    {
        var result = _validator.Validate(new GetProductsByManufactureEmailQuery(email));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(GetProductsByManufactureEmailQuery.ManufactureEmail));
    }
}
