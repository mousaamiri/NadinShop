using NadinShop.Application.Common.ResultPattern;

namespace NadinShop.Application.Tests.Common.ResultPattern;

public class ErrorTests
{
    [Fact]
    public void None_IsEmptyAndNotAnErrorType()
    {
        Assert.Equal(string.Empty, Error.None.Code);
        Assert.Equal(string.Empty, Error.None.Message);
        Assert.Equal(ErrorType.None, Error.None.ErrorType);
    }

    [Fact]
    public void NullValue_IsValidationError()
    {
        Assert.Equal("Error.NullValue", Error.NullValue.Code);
        Assert.Equal(ErrorType.Validation, Error.NullValue.ErrorType);
    }

    [Theory]
    [InlineData("Error.Validation", "Validation failed.", ErrorType.Validation)]
    [InlineData("Error.Conflict", "Conflict.", ErrorType.Conflict)]
    [InlineData("Error.Failure", "Failure.", ErrorType.Failure)]
    [InlineData("Error.NotFound", "Not found.", ErrorType.NotFound)]
    [InlineData("Error.Unauthorized", "Unauthorized.", ErrorType.Unauthorized)]
    [InlineData("Error.Forbidden", "Forbidden.", ErrorType.Forbidden)]
    [InlineData("Error.Unverified", "Unverified.", ErrorType.Verification)]
    public void Factories_CreateErrorWithExpectedType(string code, string message, ErrorType expectedType)
    {
        var error = expectedType switch
        {
            ErrorType.Validation => Error.Validation(code, message),
            ErrorType.Conflict => Error.Conflict(code, message),
            ErrorType.Failure => Error.Failure(code, message),
            ErrorType.NotFound => Error.NotFound(code, message),
            ErrorType.Unauthorized => Error.Unauthorized(code, message),
            ErrorType.Forbidden => Error.Forbidden(code, message),
            ErrorType.Verification => Error.Unverified(code, message),
            _ => throw new ArgumentOutOfRangeException(nameof(expectedType))
        };

        Assert.Equal(code, error.Code);
        Assert.Equal(message, error.Message);
        Assert.Equal(expectedType, error.ErrorType);
    }

    [Fact]
    public void Factories_UseDefaultCodeAndMessage()
    {
        Assert.Equal(new Error("Error.NotFound", "Record not found.", ErrorType.NotFound), Error.NotFound());
        Assert.Equal(new Error("Error.Unauthorized", "Unauthorized.", ErrorType.Unauthorized), Error.Unauthorized());
        Assert.Equal(new Error("Error.Forbidden", "Access denied.", ErrorType.Forbidden), Error.Forbidden());
        Assert.Equal(new Error("Error.Unverified", "Verification required.", ErrorType.Verification), Error.Unverified());
    }

    [Fact]
    public void Equals_SameCodeMessageAndType_ReturnsTrue()
    {
        var first = Error.Validation("Error.Invalid", "Invalid.");
        var second = Error.Validation("Error.Invalid", "Invalid.");

        Assert.True(first.Equals(second));
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentCodeOrMessage_ReturnsFalse()
    {
        var first = Error.Validation("Error.Invalid", "Invalid.");
        var differentCode = Error.Validation("Error.Other", "Invalid.");
        var differentMessage = Error.Validation("Error.Invalid", "Other.");

        Assert.False(first.Equals(differentCode));
        Assert.False(first.Equals(differentMessage));
        Assert.True(first != differentCode);
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var validation = Error.Validation("Error.Same", "Same.");
        var failure = Error.Failure("Error.Same", "Same.");

        Assert.NotEqual(validation, failure);
        Assert.False(validation == failure);
    }

    [Fact]
    public void Operators_HandleNulls()
    {
        Error? left = null;
        Error? right = null;
        var error = Error.Failure("Error.Failure", "Failure.");

        Assert.True(left == right);
        Assert.False(left != right);
        Assert.False(left == error);
        Assert.True(left != error);
        Assert.False(error == null);
    }

    [Fact]
    public void ToString_ReturnsCodeAndMessage()
    {
        var error = Error.NotFound("Error.ProductNotFound", "Product was not found.");

        Assert.Equal("Error.ProductNotFound:Product was not found.", error.ToString());
    }
}
