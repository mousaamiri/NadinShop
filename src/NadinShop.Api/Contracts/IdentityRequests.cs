namespace NadinShop.Api.Contracts;

public sealed record RegisterUserRequest(string Email, string Phone, string Password);

public sealed record LoginRequest(string Email, string Password);
