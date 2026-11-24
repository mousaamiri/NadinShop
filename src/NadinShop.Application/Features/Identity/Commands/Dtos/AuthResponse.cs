namespace NadinShop.Application.Features.Identity.Commands.Dtos;

public record AuthResponse(string AccessToken, string RefreshToken,DateTime ExpirationDate);
