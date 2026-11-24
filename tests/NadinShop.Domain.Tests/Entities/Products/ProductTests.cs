using NadinShop.Domain.Entities.Products;
using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Domain.Tests.Entities.Products;

public class ProductTests
{
    private const string ValidName = "Galaxy S24";
    private const string ValidPhone = "09121234567";
    private const string ValidEmail = "Test@Example.com";

    private static Product CreateProduct(bool isAvailable = true) =>
        Product.Create(ValidName, ValidPhone, ValidEmail, isAvailable);

    [Fact]
    public void Create_sets_the_given_values()
    {
        var product = Product.Create(ValidName, ValidPhone, ValidEmail, isAvailable: true);

        Assert.Equal(ValidName, product.Name.Value);
        Assert.Equal(ValidPhone, product.ManufacturePhone.Value);
        Assert.Equal("test@example.com", product.ManufactureEmail.Value);
        Assert.True(product.IsAvailable);
    }

    [Fact]
    public void Create_starts_unavailable_when_requested()
    {
        var product = CreateProduct(isAvailable: false);

        Assert.False(product.IsAvailable);
    }

    [Fact]
    public void Create_trims_the_product_name()
    {
        var product = Product.Create("  Galaxy S24  ", ValidPhone, ValidEmail, isAvailable: true);

        Assert.Equal(ValidName, product.Name.Value);
    }

    [Fact]
    public void Create_sets_produce_date_to_the_current_utc_time()
    {
        var before = DateTime.UtcNow;

        var product = Product.Create(ValidName, ValidPhone, ValidEmail, isAvailable: true);

        var after = DateTime.UtcNow;
        Assert.InRange(product.ProduceDate.Value, before, after);
    }

    [Fact]
    public void Create_generates_a_unique_id()
    {
        var first = Product.Create(ValidName, ValidPhone, ValidEmail, isAvailable: true);
        var second = Product.Create(ValidName, ValidPhone, ValidEmail, isAvailable: true);

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Create_starts_without_domain_events()
    {
        var product = Product.Create(ValidName, ValidPhone, ValidEmail, isAvailable: true);

        Assert.Empty(product.DomainEvents);
    }

    [Fact]
    public void Create_throws_when_product_name_is_empty()
    {
        Assert.Throws<ProductNameCannotBeEmptyException>(
            () => Product.Create("  ", ValidPhone, ValidEmail, isAvailable: true));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_throws_when_product_name_is_null_or_whitespace(string? name)
    {
        Assert.Throws<ProductNameCannotBeEmptyException>(
            () => Product.Create(name!, ValidPhone, ValidEmail, isAvailable: true));
    }

    [Fact]
    public void Create_throws_when_product_name_is_too_short()
    {
        Assert.Throws<ProductNameLengthIsInvalidException>(
            () => Product.Create("abc", ValidPhone, ValidEmail, isAvailable: true));
    }

    [Fact]
    public void Create_throws_when_product_name_does_not_start_with_a_latin_letter()
    {
        Assert.Throws<ProductNameFormatIsInvalidException>(
            () => Product.Create("1nvalid name", ValidPhone, ValidEmail, isAvailable: true));
    }

    [Fact]
    public void Create_throws_when_manufacture_phone_is_invalid()
    {
        Assert.Throws<ManufacturePhoneLengthIsInvalidException>(
            () => Product.Create(ValidName, "123", ValidEmail, isAvailable: true));
    }

    [Fact]
    public void Create_throws_when_manufacture_phone_is_empty()
    {
        Assert.Throws<ManufacturePhoneCannotBeEmptyException>(
            () => Product.Create(ValidName, "  ", ValidEmail, isAvailable: true));
    }

    [Fact]
    public void Create_throws_when_manufacture_phone_format_is_invalid()
    {
        Assert.Throws<ManufacturePhoneFormatIsInvalidException>(
            () => Product.Create(ValidName, "0912abc4567", ValidEmail, isAvailable: true));
    }

    [Fact]
    public void Create_throws_when_manufacture_email_is_invalid()
    {
        Assert.Throws<ManufactureEmailFormatIsInvalidException>(
            () => Product.Create(ValidName, ValidPhone, "plainaddress", isAvailable: true));
    }

    [Fact]
    public void Create_throws_when_manufacture_email_is_empty()
    {
        Assert.Throws<ManufactureEmailCannotBeEmptyException>(
            () => Product.Create(ValidName, ValidPhone, "  ", isAvailable: true));
    }

    [Fact]
    public void Rename_changes_the_product_name()
    {
        var product = CreateProduct();

        product.Rename("iPhone 15 Pro");

        Assert.Equal("iPhone 15 Pro", product.Name.Value);
    }

    [Fact]
    public void Rename_trims_the_new_name()
    {
        var product = CreateProduct();

        product.Rename("  iPhone 15 Pro  ");

        Assert.Equal("iPhone 15 Pro", product.Name.Value);
    }

    [Fact]
    public void Rename_keeps_the_name_when_the_value_is_unchanged()
    {
        var product = CreateProduct();

        product.Rename(ValidName);

        Assert.Equal(ValidName, product.Name.Value);
    }

    [Fact]
    public void Rename_throws_and_keeps_the_previous_name_when_the_new_name_is_invalid()
    {
        var product = CreateProduct();

        Assert.Throws<ProductNameFormatIsInvalidException>(() => product.Rename("1nvalid name"));
        Assert.Equal(ValidName, product.Name.Value);
    }

    [Fact]
    public void ChangeManufacturePhone_changes_the_phone()
    {
        var product = CreateProduct();

        product.ChangeManufacturePhone("09351234567");

        Assert.Equal("09351234567", product.ManufacturePhone.Value);
    }

    [Fact]
    public void ChangeManufacturePhone_throws_and_keeps_the_previous_phone_when_invalid()
    {
        var product = CreateProduct();

        Assert.Throws<ManufacturePhoneLengthIsInvalidException>(
            () => product.ChangeManufacturePhone("123"));
        Assert.Equal(ValidPhone, product.ManufacturePhone.Value);
    }

    [Fact]
    public void ChangeManufactureEmail_changes_and_normalizes_the_email()
    {
        var product = CreateProduct();

        product.ChangeManufactureEmail("  Support@NadinShop.COM  ");

        Assert.Equal("support@nadinshop.com", product.ManufactureEmail.Value);
    }

    [Fact]
    public void ChangeManufactureEmail_throws_and_keeps_the_previous_email_when_invalid()
    {
        var product = CreateProduct();

        Assert.Throws<ManufactureEmailFormatIsInvalidException>(
            () => product.ChangeManufactureEmail("plainaddress"));
        Assert.Equal("test@example.com", product.ManufactureEmail.Value);
    }

    [Fact]
    public void ChangeManufactureContact_updates_both_values()
    {
        var product = CreateProduct();

        product.ChangeManufactureContact("09351234567", "Support@NadinShop.COM");

        Assert.Equal("09351234567", product.ManufacturePhone.Value);
        Assert.Equal("support@nadinshop.com", product.ManufactureEmail.Value);
    }

    [Fact]
    public void ChangeManufactureContact_does_not_change_anything_when_phone_is_invalid()
    {
        var product = CreateProduct();

        Assert.Throws<ManufacturePhoneLengthIsInvalidException>(
            () => product.ChangeManufactureContact("123", "support@nadinshop.com"));

        Assert.Equal(ValidPhone, product.ManufacturePhone.Value);
        Assert.Equal("test@example.com", product.ManufactureEmail.Value);
    }

    [Fact]
    public void ChangeManufactureContact_does_not_change_anything_when_email_is_invalid()
    {
        var product = CreateProduct();

        Assert.Throws<ManufactureEmailFormatIsInvalidException>(
            () => product.ChangeManufactureContact("09351234567", "plainaddress"));

        Assert.Equal(ValidPhone, product.ManufacturePhone.Value);
        Assert.Equal("test@example.com", product.ManufactureEmail.Value);
    }

    [Fact]
    public void MarkAsAvailable_sets_is_available_to_true()
    {
        var product = CreateProduct(isAvailable: false);

        product.MarkAsAvailable();

        Assert.True(product.IsAvailable);
    }

    [Fact]
    public void MarkAsAvailable_is_idempotent()
    {
        var product = CreateProduct(isAvailable: true);

        product.MarkAsAvailable();

        Assert.True(product.IsAvailable);
    }

    [Fact]
    public void MarkAsUnavailable_sets_is_available_to_false()
    {
        var product = CreateProduct(isAvailable: true);

        product.MarkAsUnavailable();

        Assert.False(product.IsAvailable);
    }

    [Fact]
    public void MarkAsUnavailable_is_idempotent()
    {
        var product = CreateProduct(isAvailable: false);

        product.MarkAsUnavailable();

        Assert.False(product.IsAvailable);
    }

    [Fact]
    public void Products_with_different_ids_are_not_equal()
    {
        var first = Product.Create(ValidName, ValidPhone, ValidEmail, isAvailable: true);
        var second = Product.Create(ValidName, ValidPhone, ValidEmail, isAvailable: true);

        Assert.NotEqual(first, second);
        Assert.True(first != second);
    }

    [Fact]
    public void Product_is_equal_to_itself()
    {
        var product = CreateProduct();
        var sameReference = product;

        Assert.Equal(product, sameReference);
        Assert.True(product == sameReference);
        Assert.Equal(product.GetHashCode(), sameReference.GetHashCode());
    }
}
