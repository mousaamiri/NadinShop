using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Application.Tests.Common.ResultPattern;

public class ResultTests
{
    [Fact]
    public void Success_ReturnsSuccessfulResultWithoutError()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_ReturnsFailedResultWithGivenError()
    {
        var error = Error.Validation("Error.Invalid", "Value is invalid.");

        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Constructor_SuccessWithError_Throws()
    {
        var error = Error.Failure("Error.Failure", "Something went wrong.");

        Assert.Throws<InvalidOperationException>(() => new TestResult(true, error));
    }

    [Fact]
    public void Constructor_FailureWithoutError_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => new TestResult(false, Error.None));
    }

    [Fact]
    public void SuccessWithValue_ExposesValue()
    {
        var result = Result.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void FailureWithValue_AccessingValue_Throws()
    {
        var result = Result.Failure<int>(Error.NotFound());

        Assert.True(result.IsFailure);
        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public void Create_WithValue_ReturnsSuccess()
    {
        var result = Result.Create("value");

        Assert.True(result.IsSuccess);
        Assert.Equal("value", result.Value);
    }

    [Fact]
    public void Create_WithNull_ReturnsNullValueFailure()
    {
        var result = Result.Create<string?>(null);

        Assert.True(result.IsFailure);
        Assert.Equal(Error.NullValue, result.Error);
    }

    [Fact]
    public void ImplicitConversion_FromValue_ReturnsSuccess()
    {
        Result<string> result = "value";

        Assert.True(result.IsSuccess);
        Assert.Equal("value", result.Value);
    }

    [Fact]
    public void ImplicitConversion_FromError_ReturnsFailure()
    {
        var error = Error.Conflict("Error.Conflict", "Already exists.");

        Result result = error;

        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    private sealed class TestResult(bool isSuccess, Error error) : Result(isSuccess, error);
}
