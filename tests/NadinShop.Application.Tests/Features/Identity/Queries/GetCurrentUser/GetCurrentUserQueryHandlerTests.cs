using Moq;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Identity.Queries.GetCurrentUser;

namespace NadinShop.Application.Tests.Features.Identity.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandlerTests
{
    private readonly CancellationToken _token;
    private readonly CurrentUser _currentUser;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryHandlerTests()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        _token = cancellationTokenSource.Token;
        _currentUser = new CurrentUser(Guid.NewGuid(), "test@example.com", "09121234567");

        _currentUserService = new Mock<ICurrentUserService>();
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(_token))
            .ReturnsAsync(_currentUser);

        _handler = new GetCurrentUserQueryHandler(_currentUserService.Object);
    }

    [Fact]
    public async Task Handle_WhenUserIsAuthenticated_ReturnsCurrentUser()
    {
        var result = await _handler.Handle(new GetCurrentUserQuery(), _token);

        Assert.True(result.IsSuccess);
        Assert.Equal(_currentUser, result.Value);
        _currentUserService.Verify(s => s.GetCurrentUserAsync(_token), Times.Once);
        _currentUserService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrentUser?)null);

        var result = await _handler.Handle(new GetCurrentUserQuery(), _token);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Unauthorized, result.Error.ErrorType);
        Assert.Equal("Error.Unauthenticated", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_DoesNotExposeValue()
    {
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrentUser?)null);

        var result = await _handler.Handle(new GetCurrentUserQuery(), _token);

        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }
}
