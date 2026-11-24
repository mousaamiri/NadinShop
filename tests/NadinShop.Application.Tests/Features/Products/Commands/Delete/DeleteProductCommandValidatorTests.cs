using NadinShop.Application.Features.Products.Commands.Delete;

namespace NadinShop.Application.Tests.Features.Products.Commands.Delete;

public class DeleteProductCommandValidatorTests
{
    private readonly DeleteProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidId_ReturnsValid()
    {
        var result = _validator.Validate(new DeleteProductCommand(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyId_ReturnsIdError()
    {
        var result = _validator.Validate(new DeleteProductCommand(Guid.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(DeleteProductCommand.Id));
    }
}
