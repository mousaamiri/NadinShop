using Microsoft.AspNetCore.Identity;

namespace NadinShop.Infrastructure.Data.Entities;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string NormalizedPhone { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
