namespace NadinShop.Application.Common.ResultPattern;

public class Result
{
    #region Constructor

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("A successful result cannot contain an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("A failed result must contain an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    #endregion

    #region Properties

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    #endregion

    #region Factory methods

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    public static Result WithErrors(Error[] errors)
    {
        if (errors.Length == 0) return Success();
        // If multiple errors, take the first one or merge it
        return errors[0];
    }
    #endregion

    #region Operators

    public static implicit operator Result(Error error) => Failure(error);

    #endregion
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failed result cannot be accessed.");
    public new static Result<TValue> WithErrors(Error[] errors)
    {
        if (errors.Length == 0) return Success<TValue>(default!);
        return errors[0];
    }
    public static implicit operator Result<TValue>(TValue? value) => Create(value);
    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
}
