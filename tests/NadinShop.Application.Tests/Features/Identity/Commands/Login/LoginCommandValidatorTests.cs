using NadinShop.Application.Features.Identity.Commands.Login;

namespace NadinShop.Application.Tests.Features.Identity.Commands.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ReturnsValid()
    {
        var result = _validator.Validate(new LoginCommand("user@example.com", "P@ssw0rd!"));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidEmail_ReturnsEmailError(string email)
    {
        var result = _validator.Validate(new LoginCommand(email, "P@ssw0rd!"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(LoginCommand.Email));
    }

    [Fact]
    public void Validate_WithEmptyPassword_ReturnsPasswordError()
    {
        var result = _validator.Validate(new LoginCommand("user@example.com", string.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(LoginCommand.Password));
    }
}
