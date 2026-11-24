namespace NadinShop.Application.Common.ResultPattern;

public class Error(string code, string message, ErrorType errorType) : IEquatable<Error>
{
    #region Common Errors
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided.", ErrorType.Validation);

    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
    public static Error Failure(string code, string message) => new(code, message, ErrorType.Failure);

    public static Error NotFound(string code = "Error.NotFound", string message = "Record not found.")
        => new(code, message, ErrorType.NotFound);
    public static Error Unauthorized(string code = "Error.Unauthorized", string message = "Unauthorized.")
        => new(code, message, ErrorType.Unauthorized);
    public static Error Forbidden(string code = "Error.Forbidden", string message = "Access denied.")
        => new(code, message, ErrorType.Forbidden);
    public static Error Unverified(string code = "Error.Unverified", string message = "Verification required.")
        => new(code, message, ErrorType.Verification);
    #endregion

    #region Fields

    public string Code => code;
    public string Message => message;
    public ErrorType ErrorType => errorType;

    #endregion
    #region Operators

    public static bool operator ==(Error? left, Error? right)
    {
        if (left is null && right is null)
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }

    public static bool operator !=(Error? left, Error? right) => !(left == right);

    #endregion

    #region Base methods

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return obj.GetType() == GetType() && Equals((Error)obj);
    }

    public override int GetHashCode() => HashCode.Combine(code, message);
    public override string ToString() => $"{code}:{message}";
    public bool Equals(Error? other)
    {
        if (other is null)
            return false;

        return string.Equals(code, other.Code, StringComparison.Ordinal) && string.Equals(message, other.Message, StringComparison.Ordinal) && errorType == other.ErrorType;
    }

    #endregion

}
