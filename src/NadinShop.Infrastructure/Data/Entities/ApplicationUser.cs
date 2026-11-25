namespace NadinShop.Infrastructure.Data.Entities;

public sealed class ApplicationUser
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string NormalizedEmail { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string NormalizedPhone { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
