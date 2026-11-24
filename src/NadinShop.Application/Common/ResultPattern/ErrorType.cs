namespace NadinShop.Application.Common.ResultPattern;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Conflict,
    Failure,
    Unauthorized,
    Forbidden,
    Verification
}
