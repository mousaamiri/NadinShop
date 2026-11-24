using MediatR;
using Moq;
using NadinShop.Application.Common.Behaviors;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Application.Tests.Common.Behaviors;

public class UnitOfWorkBehaviorTests
{
    [Fact]
    public async Task Handle_WhenCommandSucceeds_SavesChangesOnceWithCancellationToken()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork
            .Setup(u => u.SaveChangesAsync(token))
            .ReturnsAsync(1);
        var behavior = new UnitOfWorkBehavior<TestCommand, Result>(unitOfWork.Object);
        var expected = Result.Success();
        RequestHandlerDelegate<Result> next = ct =>
        {
            Assert.Equal(token, ct);
            return Task.FromResult(expected);
        };

        var response = await behavior.Handle(new TestCommand(), next, token);

        Assert.Same(expected, response);
        unitOfWork.Verify(u => u.SaveChangesAsync(token), Times.Once);
        unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenCommandFails_DoesNotSaveChanges()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var behavior = new UnitOfWorkBehavior<TestCommand, Result>(unitOfWork.Object);
        var error = Error.Validation("TestCommand.Invalid", "Command is invalid.");
        RequestHandlerDelegate<Result> next = _ => Task.FromResult(Result.Failure(error));

        var response = await behavior.Handle(new TestCommand(), next, CancellationToken.None);

        Assert.True(response.IsFailure);
        Assert.Equal(error, response.Error);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestIsNotCommand_DoesNotSaveChanges()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var behavior = new UnitOfWorkBehavior<TestQuery, Result>(unitOfWork.Object);
        RequestHandlerDelegate<Result> next = _ => Task.FromResult(Result.Success());

        var response = await behavior.Handle(new TestQuery(), next, CancellationToken.None);

        Assert.True(response.IsSuccess);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCommandReturnsGenericResult_SavesChangesOnSuccess()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var behavior = new UnitOfWorkBehavior<TestValueCommand, Result<int>>(unitOfWork.Object);
        RequestHandlerDelegate<Result<int>> next = _ => Task.FromResult(Result.Success(42));

        var response = await behavior.Handle(new TestValueCommand(), next, CancellationToken.None);

        Assert.True(response.IsSuccess);
        Assert.Equal(42, response.Value);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSaveChangesThrows_PropagatesException()
    {
        var exception = new InvalidOperationException("Database is unavailable.");
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
        var behavior = new UnitOfWorkBehavior<TestCommand, Result>(unitOfWork.Object);
        RequestHandlerDelegate<Result> next = _ => Task.FromResult(Result.Success());

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(
            () => behavior.Handle(new TestCommand(), next, CancellationToken.None));

        Assert.Same(exception, thrown);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private sealed record TestCommand : IRequest<Result>;

    private sealed record TestQuery : IRequest<Result>;

    private sealed record TestValueCommand : IRequest<Result<int>>;
}
