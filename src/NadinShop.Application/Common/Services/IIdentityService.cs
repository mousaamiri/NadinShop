using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Features.Users.Commands.Dtos;

namespace NadinShop.Application.Common.Services;

public interface IIdentityService
{
    Task<Result<Guid>> CreateUserAsync(string email, string phone, string password,CancellationToken ct);
    Task<Result<AuthUser>> ValidateCredentialsAsync(string email, string password,CancellationToken ct);
}
