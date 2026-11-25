using NadinShop.Application.Features.Products.Commands.Create;

namespace NadinShop.Application.Tests.Features.Products.Commands.Create;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ReturnsValid()
    {
        var command = new CreateProductCommand("Galaxy S24", DateTime.UtcNow.AddDays(-1));

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithPersianName_ReturnsValid()
    {
        var command = new CreateProductCommand("گوشی سامسونگ گلکسی", DateTime.UtcNow.AddDays(-1));

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abcd")]
    [InlineData("1Galaxy S24")]
    [InlineData("Galaxy--S24")]
    public void Validate_WithInvalidName_ReturnsNameError(string name)
    {
        var command = new CreateProductCommand(name, DateTime.UtcNow.AddDays(-1));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Fact]
    public void Validate_WithDefaultProduceDate_ReturnsProduceDateError()
    {
        var command = new CreateProductCommand("Galaxy S24", default);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductCommand.ProduceDate));
    }

    [Fact]
    public void Validate_WithFutureProduceDate_ReturnsProduceDateError()
    {
        var command = new CreateProductCommand("Galaxy S24", DateTime.UtcNow.AddDays(1));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductCommand.ProduceDate));
    }
}
