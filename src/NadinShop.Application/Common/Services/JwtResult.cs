namespace NadinShop.Application.Common.Services;

public sealed record JwtResult(string UserId,string AccessToken, string RefreshToken, DateTime ExpirationDate);
