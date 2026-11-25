using NadinShop.Application.Common.Services;
using NadinShop.Domain.Entities.Products;

namespace NadinShop.Application.Features.Products.Common;

internal static class ProductOwnership
{
    public static bool IsOwnedBy(Product product, CurrentUser currentUser)
        => IsSameManufactureEmail(product.ManufactureEmail.Value, currentUser.Email);

    public static bool IsSameManufactureEmail(string manufactureEmail, string currentUserEmail)
        => string.Equals(
            NormalizeEmail(manufactureEmail),
            NormalizeEmail(currentUserEmail),
            StringComparison.Ordinal);

    private static string NormalizeEmail(string email)
        => email.Trim().ToLowerInvariant();
}
