using NadinShop.Application.Features.Products.Commands.Update;

namespace NadinShop.Application.Tests.Features.Products.Commands.Update;

public class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ReturnsValid()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Galaxy S24", IsAvailable: true);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyId_ReturnsIdError()
    {
        var command = new UpdateProductCommand(Guid.Empty, "Galaxy S24", IsAvailable: true);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProductCommand.Id));
    }

    [Theory]
    [InlineData("")]
    [InlineData("abcd")]
    [InlineData("1Galaxy S24")]
    public void Validate_WithInvalidName_ReturnsNameError(string name)
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), name, IsAvailable: true);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProductCommand.Name));
    }
}
