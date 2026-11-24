using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NadinShop.Application.Common.Behaviors;
using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Application.Tests.Common.Behaviors;

public class ValidationPipelineBehaviorTests
{
    [Fact]
    public async Task Handle_WithoutValidators_InvokesNextWithCancellationToken()
    {
        var behavior = new ValidationPipelineBehavior<ValidatedRequest, Result<Guid>>(
            Array.Empty<IValidator<ValidatedRequest>>());
        var request = new ValidatedRequest("user@example.com", "Nadin");
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var expected = Result.Success(Guid.NewGuid());
        var nextCallCount = 0;
        RequestHandlerDelegate<Result<Guid>> next = ct =>
        {
            nextCallCount++;
            Assert.Equal(token, ct);
            return Task.FromResult(expected);
        };

        var response = await behavior.Handle(request, next, token);

        Assert.Equal(1, nextCallCount);
        Assert.Same(expected, response);
    }

    [Fact]
    public async Task Handle_WithPassingValidator_InvokesNextAndReturnsItsResponse()
    {
        var request = new ValidatedRequest("user@example.com", "Nadin");
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var validator = CreateValidator<ValidatedRequest>();
        var behavior = new ValidationPipelineBehavior<ValidatedRequest, Result<Guid>>([validator.Object]);
        var expected = Result.Success(Guid.NewGuid());
        var nextCallCount = 0;
        RequestHandlerDelegate<Result<Guid>> next = _ =>
        {
            nextCallCount++;
            return Task.FromResult(expected);
        };

        var response = await behavior.Handle(request, next, token);

        Assert.Equal(1, nextCallCount);
        Assert.Same(expected, response);
        validator.Verify(
            v => v.ValidateAsync(
                It.Is<IValidationContext>(context => ReferenceEquals(context.InstanceToValidate, request)),
                token),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithFailingValidator_ReturnsValidationErrorAndSkipsNext()
    {
        var validator = CreateValidator<ValidatedRequest>(
            new ValidationFailure("Email", "Email is required."));
        var behavior = new ValidationPipelineBehavior<ValidatedRequest, Result<Guid>>([validator.Object]);
        var nextCallCount = 0;
        RequestHandlerDelegate<Result<Guid>> next = _ =>
        {
            nextCallCount++;
            return Task.FromResult(Result.Success(Guid.NewGuid()));
        };

        var response = await behavior.Handle(
            new ValidatedRequest(string.Empty, "Nadin"),
            next,
            CancellationToken.None);

        Assert.True(response.IsFailure);
        Assert.Equal(0, nextCallCount);
        Assert.Equal(ErrorType.Validation, response.Error.ErrorType);
        Assert.Equal("ValidatedRequest.Email", response.Error.Code);
        Assert.Equal("Email is required.", response.Error.Message);
    }

    [Fact]
    public async Task Handle_WithFailingValidatorForNonGenericResult_ReturnsValidationFailure()
    {
        var validator = CreateValidator<PlainRequest>(
            new ValidationFailure("Email", "Email is required."));
        var behavior = new ValidationPipelineBehavior<PlainRequest, Result>([validator.Object]);
        var nextCallCount = 0;
        RequestHandlerDelegate<Result> next = _ =>
        {
            nextCallCount++;
            return Task.FromResult(Result.Success());
        };

        var response = await behavior.Handle(new PlainRequest(string.Empty), next, CancellationToken.None);

        Assert.True(response.IsFailure);
        Assert.Equal(0, nextCallCount);
        Assert.Equal("PlainRequest.Email", response.Error.Code);
        Assert.Equal("Email is required.", response.Error.Message);
    }

    [Fact]
    public async Task Handle_WithMultipleFailingValidators_ReturnsFirstErrorInValidatorOrder()
    {
        var emailValidator = CreateValidator<ValidatedRequest>(
            new ValidationFailure("Email", "Email is required."));
        var nameValidator = CreateValidator<ValidatedRequest>(
            new ValidationFailure("Name", "Name is required."));
        var behavior = new ValidationPipelineBehavior<ValidatedRequest, Result<Guid>>(
            [emailValidator.Object, nameValidator.Object]);
        RequestHandlerDelegate<Result<Guid>> next = _ => Task.FromResult(Result.Success(Guid.NewGuid()));

        var response = await behavior.Handle(
            new ValidatedRequest(string.Empty, string.Empty),
            next,
            CancellationToken.None);

        Assert.True(response.IsFailure);
        Assert.Equal("ValidatedRequest.Email", response.Error.Code);
        Assert.Equal("Email is required.", response.Error.Message);
        emailValidator.Verify(
            v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()),
            Times.Once);
        nameValidator.Verify(
            v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithDuplicateErrorCodes_KeepsOnlyFirstFailure()
    {
        var firstValidator = CreateValidator<ValidatedRequest>(
            new ValidationFailure("Email", "First message."));
        var secondValidator = CreateValidator<ValidatedRequest>(
            new ValidationFailure("Email", "Second message."));
        var behavior = new ValidationPipelineBehavior<ValidatedRequest, Result<Guid>>(
            [firstValidator.Object, secondValidator.Object]);
        RequestHandlerDelegate<Result<Guid>> next = _ => Task.FromResult(Result.Success(Guid.NewGuid()));

        var response = await behavior.Handle(
            new ValidatedRequest(string.Empty, "Nadin"),
            next,
            CancellationToken.None);

        Assert.True(response.IsFailure);
        Assert.Equal("ValidatedRequest.Email", response.Error.Code);
        Assert.Equal("First message.", response.Error.Message);
    }

    private static Mock<IValidator<T>> CreateValidator<T>(params ValidationFailure[] failures)
    {
        var validator = new Mock<IValidator<T>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        return validator;
    }

    public sealed record ValidatedRequest(string Email, string Name) : IRequest<Result<Guid>>;

    public sealed record PlainRequest(string Email) : IRequest<Result>;
}
