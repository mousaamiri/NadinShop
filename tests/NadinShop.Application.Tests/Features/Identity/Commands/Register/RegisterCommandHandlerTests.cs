using Moq;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Identity.Commands.Register;

namespace NadinShop.Application.Tests.Features.Identity.Commands.Register;

public class RegisterCommandHandlerTests
{
    private RegisterCommand _command;
    private readonly CancellationToken _token;
    private readonly Guid _expectedUserId;
    private readonly Mock<IIdentityService> _identityService;
    private RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _command = new RegisterCommand("new-user@example.com", "09121234567", "P@ssw0rd!");
        using var cancellationTokenSource = new CancellationTokenSource();
        _token = cancellationTokenSource.Token;
        _expectedUserId = Guid.NewGuid();
        _identityService = new Mock<IIdentityService>();
        _identityService
            .Setup(s => s.CreateUserAsync(_command.Email, _command.Phone, _command.Password, _token))
            .ReturnsAsync(Result.Success(_expectedUserId));
        _handler = new RegisterCommandHandler(_identityService.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsCreatedUserId()
    {
        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsSuccess);
        Assert.Equal(_expectedUserId, result.Value);
        _identityService.Verify(
            s => s.CreateUserAsync(_command.Email, _command.Phone, _command.Password, _token),
            Times.Once);
        _identityService.Verify(
            s => s.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _identityService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenUserCreationFails_ReturnsIdentityError()
    {
        _command = new RegisterCommand("existing@example.com", "09121234567", "P@ssw0rd!");
        var error = Error.Conflict("Error.EmailAlreadyExists", "Email is already registered.");
        _identityService
            .Setup(s => s.CreateUserAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Guid>(error));
        _handler = new RegisterCommandHandler(_identityService.Object);

        var result = await _handler.Handle(_command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
        Assert.Equal(ErrorType.Conflict, result.Error.ErrorType);
        _identityService.Verify(
            s => s.CreateUserAsync(_command.Email, _command.Phone, _command.Password, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserCreationFails_DoesNotExposeValue()
    {
        _identityService
            .Setup(s => s.CreateUserAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Guid>(
                Error.Validation("RegisterCommand.InvalidEmail", "Email is invalid.")));
        var handler = new RegisterCommandHandler(_identityService.Object);

        var result = await handler.Handle(
            new RegisterCommand("invalid", "09121234567", "P@ssw0rd!"),
            CancellationToken.None);

        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }
}
