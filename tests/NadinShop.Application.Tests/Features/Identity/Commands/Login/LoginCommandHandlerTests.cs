using Moq;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Identity.Commands.Dtos;
using NadinShop.Application.Features.Identity.Commands.Login;

namespace NadinShop.Application.Tests.Features.Identity.Commands.Login;

public class LoginCommandHandlerTests
{
    private static readonly AuthUser TestUser = new(Guid.NewGuid(), "user@example.com");
    private static readonly JwtResult TestJwt = new(
        TestUser.UserId.ToString(),
        "access-token",
        "refresh-token",
        new DateTime(2026, 11, 24, 12, 0, 0, DateTimeKind.Utc));

    private readonly LoginCommand _command;
    private readonly CancellationToken _token;
    private readonly Mock<IIdentityService> _identityService;
    private readonly Mock<IJwTokenService> _jwtTokenService;
    public LoginCommandHandlerTests()
    {

        _command = new LoginCommand("user@example.com", "P@ssw0rd!");
        using var cancellationTokenSource = new CancellationTokenSource();
        _token = cancellationTokenSource.Token;
        _identityService = new Mock<IIdentityService>();
        _jwtTokenService = new Mock<IJwTokenService>();
        _identityService
            .Setup(s => s.ValidateCredentialsAsync(_command.Email, _command.Password, _token))
            .ReturnsAsync(Result.Success(TestUser));
        _jwtTokenService
            .Setup(s => s.GenerateAccessToken(TestUser, _token))
            .ReturnsAsync(TestJwt);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsGeneratedToken()
    {
        var handler = new LoginCommandHandler(_identityService.Object, _jwtTokenService.Object);

        var result = await handler.Handle(_command, _token);

        Assert.True(result.IsSuccess);
        Assert.Equal(TestJwt, result.Value);
        _identityService.Verify(s => s.ValidateCredentialsAsync(_command.Email, _command.Password, _token), Times.Once);
        _jwtTokenService.Verify(s => s.GenerateAccessToken(TestUser, _token), Times.Once);
        _identityService.VerifyNoOtherCalls();
        _jwtTokenService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WithInvalidCredentials_ReturnsIdentityErrorWithoutGeneratingToken()
    {
        var error = Error.Unauthorized("Error.InvalidCredentials", "Invalid email or password.");

        _identityService
            .Setup(s => s.ValidateCredentialsAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<AuthUser>(error));
        var handler = new LoginCommandHandler(_identityService.Object, _jwtTokenService.Object);

        var result = await handler.Handle(new LoginCommand("user@example.com", "wrong-password"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
        Assert.Equal(ErrorType.Unauthorized, result.Error.ErrorType);
        _identityService.Verify(
            s => s.ValidateCredentialsAsync("user@example.com", "wrong-password", It.IsAny<CancellationToken>()),
            Times.Once);
        _jwtTokenService.Verify(
            s => s.GenerateAccessToken(It.IsAny<AuthUser>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidCredentials_DoesNotExposeValue()
    {
        _identityService
            .Setup(s => s.ValidateCredentialsAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<AuthUser>(
                Error.Unauthorized("Error.InvalidCredentials", "Invalid email or password.")));
        var handler = new LoginCommandHandler(_identityService.Object, _jwtTokenService.Object);

        var result = await handler.Handle(new LoginCommand("user@example.com", "wrong-password"), CancellationToken.None);

        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }
}
