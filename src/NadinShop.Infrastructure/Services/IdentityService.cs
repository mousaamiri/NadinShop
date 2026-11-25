using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Identity.Commands.Dtos;
using NadinShop.Infrastructure.Data;
using NadinShop.Infrastructure.Data.Entities;

namespace NadinShop.Infrastructure.Services;

public sealed class IdentityService(
    NadinShopDbContext dbContext,
    IPasswordHasher<ApplicationUser> passwordHasher) : IIdentityService
{
    public async Task<Result<Guid>> CreateUserAsync(string email, string phone, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Error.Validation("Error.EmailRequired", "Email is required.");

        if (string.IsNullOrWhiteSpace(phone))
            return Error.Validation("Error.PhoneRequired", "Phone number is required.");

        if (string.IsNullOrWhiteSpace(password))
            return Error.Validation("Error.PasswordRequired", "Password is required.");

        var normalizedEmail = NormalizeEmail(email);
        var normalizedPhone = NormalizePhone(phone);

        if (await dbContext.Users.AnyAsync(user => user.NormalizedEmail == normalizedEmail, ct))
            return Error.Conflict("Error.EmailAlreadyExists", "Email is already registered.");

        if (await dbContext.Users.AnyAsync(user => user.NormalizedPhone == normalizedPhone, ct))
            return Error.Conflict("Error.PhoneAlreadyExists", "Phone number is already registered.");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email.Trim(),
            NormalizedEmail = normalizedEmail,
            Phone = phone.Trim(),
            NormalizedPhone = normalizedPhone,
            CreatedAtUtc = DateTime.UtcNow
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        return Result.Success(user.Id);
    }

    public async Task<Result<AuthUser>> ValidateCredentialsAsync(string email, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return InvalidCredentials();

        var normalizedEmail = NormalizeEmail(email);

        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(candidate => candidate.NormalizedEmail == normalizedEmail, ct);

        if (user is null)
            return InvalidCredentials();

        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verification == PasswordVerificationResult.Failed)
            return InvalidCredentials();

        return Result.Success(new AuthUser(user.Id, user.Email));
    }

    private static Error InvalidCredentials()
        => Error.Unauthorized("Error.InvalidCredentials", "Invalid email or password.");

    private static string NormalizeEmail(string email)
        => email.Trim().ToLowerInvariant();

    private static string NormalizePhone(string phone)
        => new(phone.Where(char.IsDigit).ToArray());
}
