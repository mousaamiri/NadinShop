using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Identity.Commands.Dtos;
using NadinShop.Infrastructure.Data.Entities;

namespace NadinShop.Infrastructure.Services;

public sealed class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    public async Task<Result<Guid>> CreateUserAsync(string email, string phone, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Error.Validation("Error.EmailRequired", "Email is required.");

        if (string.IsNullOrWhiteSpace(phone))
            return Error.Validation("Error.PhoneRequired", "Phone number is required.");

        if (string.IsNullOrWhiteSpace(password))
            return Error.Validation("Error.PasswordRequired", "Password is required.");

        var normalizedPhone = NormalizePhone(phone);

        if (await userManager.FindByEmailAsync(email.Trim()) is not null)
            return Error.Conflict("Error.EmailAlreadyExists", "Email is already registered.");

        if (await userManager.Users.AnyAsync(user => user.NormalizedPhone == normalizedPhone, ct))
            return Error.Conflict("Error.PhoneAlreadyExists", "Phone number is already registered.");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email.Trim(),
            Email = email.Trim(),
            PhoneNumber = phone.Trim(),
            NormalizedPhone = normalizedPhone,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var message = string.Join(
                " ",
                createResult.Errors.Select(error => error.Description));

            return Error.Validation("Error.UserCreationFailed", message);
        }

        return Result.Success(user.Id);
    }

    public async Task<Result<AuthUser>> ValidateCredentialsAsync(string email, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return InvalidCredentials();

        var user = await userManager.FindByEmailAsync(email.Trim());

        if (user is null)
            return InvalidCredentials();

        if (!await userManager.CheckPasswordAsync(user, password))
            return InvalidCredentials();

        return Result.Success(new AuthUser(user.Id, user.Email ?? string.Empty));
    }

    private static Error InvalidCredentials()
        => Error.Unauthorized("Error.InvalidCredentials", "Invalid email or password.");

    private static string NormalizePhone(string phone)
        => new(phone.Where(char.IsDigit).ToArray());
}
