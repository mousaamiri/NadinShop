using NadinShop.Application.Common.Services;
using NadinShop.Domain.Entities.Products;

namespace NadinShop.Application.Features.Products.Common;

internal static class ProductOwnership
{
    public static bool IsOwnedBy(Product product, CurrentUser currentUser)
        => IsContactOf(product.ManufacturePhone.Value, product.ManufactureEmail.Value, currentUser);

    public static bool IsContactOf(string phone, string email, CurrentUser currentUser)
        => string.Equals(NormalizePhone(phone), NormalizePhone(currentUser.Phone), StringComparison.Ordinal) && string.Equals(NormalizeEmail(email), NormalizeEmail(currentUser.Email), StringComparison.Ordinal);

    private static string NormalizePhone(string phone)
        => new(phone.Where(char.IsDigit).ToArray());

    private static string NormalizeEmail(string email)
        => email.Trim().ToLowerInvariant();
}
