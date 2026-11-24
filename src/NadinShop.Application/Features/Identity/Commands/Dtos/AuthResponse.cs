namespace NadinShop.Application.Features.Users.Commands.Dtos;

public record AuthResponse(string AccessToken, string RefreshToken,DateTime ExpirationDate);
public record AuthUser(Guid UserId,string Email);
