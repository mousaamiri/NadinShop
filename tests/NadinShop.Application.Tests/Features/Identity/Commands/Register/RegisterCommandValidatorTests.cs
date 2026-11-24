using NadinShop.Application.Features.Identity.Commands.Register;

namespace NadinShop.Application.Tests.Features.Identity.Commands.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ReturnsValid()
    {
        var result = _validator.Validate(
            new RegisterCommand("new-user@example.com", "09121234567", "P@ssw0rd!"));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidEmail_ReturnsEmailError(string email)
    {
        var result = _validator.Validate(new RegisterCommand(email, "09121234567", "P@ssw0rd!"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterCommand.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("0912abc4567")]
    public void Validate_WithInvalidPhone_ReturnsPhoneError(string phone)
    {
        var result = _validator.Validate(new RegisterCommand("new-user@example.com", phone, "P@ssw0rd!"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterCommand.Phone));
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    public void Validate_WithInvalidPassword_ReturnsPasswordError(string password)
    {
        var result = _validator.Validate(new RegisterCommand("new-user@example.com", "09121234567", password));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterCommand.Password));
    }
}
