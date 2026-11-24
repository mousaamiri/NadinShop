namespace NadinShop.Application.Common.Services;

public sealed record CurrentUser(Guid UserId, string Email, string Phone);
