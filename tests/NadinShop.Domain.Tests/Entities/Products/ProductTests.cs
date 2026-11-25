using NadinShop.Domain.Entities.Products;
using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Domain.Tests.Entities.Products;

public class ProductTests
{
    private const string ValidName = "Galaxy S24";
    private const string ValidPhone = "09121234567";
    private const string ValidEmail = "Test@Example.com";
    private static readonly DateTime ValidProduceDate = new(2024, 8, 1);

    private static Product CreateProduct(bool isAvailable = true) =>
        Product.Create(ValidName, ValidPhone, ValidEmail, ValidProduceDate, isAvailable);

    [Fact]
    public void Create_ValidValues_SetsValues()
    {
        var product = Product.Create(ValidName, ValidPhone, ValidEmail, ValidProduceDate, isAvailable: true);

        Assert.Equal(ValidName, product.Name.Value);
        Assert.Equal(ValidPhone, product.ManufacturePhone.Value);
        Assert.Equal("test@example.com", product.ManufactureEmail.Value);
        Assert.Equal(ValidProduceDate, product.ProduceDate.Value);
        Assert.True(product.IsAvailable);
    }

    [Fact]
    public void Create_UnavailableRequested_SetsIsAvailableFalse()
    {
        var product = CreateProduct(isAvailable: false);

        Assert.False(product.IsAvailable);
    }

    [Fact]
    public void Create_NameWithSurroundingWhitespace_TrimsName()
    {
        var product = Product.Create("  Galaxy S24  ", ValidPhone, ValidEmail, ValidProduceDate, isAvailable: true);

        Assert.Equal(ValidName, product.Name.Value);
    }

    [Fact]
    public void Create_WithProduceDate_SetsProduceDate()
    {
        var product = Product.Create(ValidName, ValidPhone, ValidEmail, ValidProduceDate, isAvailable: true);

        Assert.Equal(ValidProduceDate, product.ProduceDate.Value);
    }

    [Fact]
    public void Create_ValidValues_AssignsUniqueId()
    {
        var first = Product.Create(ValidName, ValidPhone, ValidEmail, ValidProduceDate, isAvailable: true);
        var second = Product.Create(ValidName, ValidPhone, ValidEmail, ValidProduceDate, isAvailable: true);

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Create_ValidValues_StartsWithoutDomainEvents()
    {
        var product = Product.Create(ValidName, ValidPhone, ValidEmail, ValidProduceDate, isAvailable: true);

        Assert.Empty(product.DomainEvents);
    }

    [Fact]
    public void Create_WhitespaceProductName_ThrowsProductNameCannotBeEmptyException()
    {
        Assert.Throws<ProductNameCannotBeEmptyException>(
            () => Product.Create("  ", ValidPhone, ValidEmail, ValidProduceDate, isAvailable: true));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrWhitespaceProductName_ThrowsProductNameCannotBeEmptyException(string? name)
    {
        Assert.Throws<ProductNameCannotBeEmptyException>(
            () => Product.Create(name!, ValidPhone, ValidEmail, ValidProduceDate, isAvailable: true));
    }

    [Fact]
    public void Create_ShortProductName_ThrowsProductNameLengthIsInvalidException()
    {
        Assert.Throws<ProductNameLengthIsInvalidException>(
            () => Product.Create("abc", ValidPhone, ValidEmail, ValidProduceDate, isAvailable: true));
    }

    [Fact]
    public void Create_ProductNameNotStartingWithLatinLetter_ThrowsProductNameFormatIsInvalidException()
    {
        Assert.Throws<ProductNameFormatIsInvalidException>(
            () => Product.Create("1nvalid name", ValidPhone, ValidEmail, ValidProduceDate, isAvailable: true));
    }

    [Fact]
    public void Create_InvalidManufacturePhoneLength_ThrowsManufacturePhoneLengthIsInvalidException()
    {
        Assert.Throws<ManufacturePhoneLengthIsInvalidException>(
            () => Product.Create(ValidName, "123", ValidEmail, ValidProduceDate, isAvailable: true));
    }

    [Fact]
    public void Create_EmptyManufacturePhone_ThrowsManufacturePhoneCannotBeEmptyException()
    {
        Assert.Throws<ManufacturePhoneCannotBeEmptyException>(
            () => Product.Create(ValidName, "  ", ValidEmail, ValidProduceDate, isAvailable: true));
    }

    [Fact]
    public void Create_InvalidManufacturePhoneFormat_ThrowsManufacturePhoneFormatIsInvalidException()
    {
        Assert.Throws<ManufacturePhoneFormatIsInvalidException>(
            () => Product.Create(ValidName, "0912abc4567", ValidEmail, ValidProduceDate, isAvailable: true));
    }

    [Fact]
    public void Create_InvalidManufactureEmailFormat_ThrowsManufactureEmailFormatIsInvalidException()
    {
        Assert.Throws<ManufactureEmailFormatIsInvalidException>(
            () => Product.Create(ValidName, ValidPhone, "plainaddress", ValidProduceDate, isAvailable: true));
    }

    [Fact]
    public void Create_EmptyManufactureEmail_ThrowsManufactureEmailCannotBeEmptyException()
    {
        Assert.Throws<ManufactureEmailCannotBeEmptyException>(
            () => Product.Create(ValidName, ValidPhone, "  ", ValidProduceDate, isAvailable: true));
    }

    [Fact]
    public void Rename_ValidName_ChangesName()
    {
        var product = CreateProduct();

        product.Rename("iPhone 15 Pro");

        Assert.Equal("iPhone 15 Pro", product.Name.Value);
    }

    [Fact]
    public void Rename_NameWithSurroundingWhitespace_TrimsName()
    {
        var product = CreateProduct();

        product.Rename("  iPhone 15 Pro  ");

        Assert.Equal("iPhone 15 Pro", product.Name.Value);
    }

    [Fact]
    public void Rename_SameName_KeepsName()
    {
        var product = CreateProduct();

        product.Rename(ValidName);

        Assert.Equal(ValidName, product.Name.Value);
    }

    [Fact]
    public void Rename_InvalidName_ThrowsAndKeepsPreviousName()
    {
        var product = CreateProduct();

        Assert.Throws<ProductNameFormatIsInvalidException>(() => product.Rename("1nvalid name"));
        Assert.Equal(ValidName, product.Name.Value);
    }

    [Fact]
    public void ChangeManufacturePhone_ValidPhone_ChangesPhone()
    {
        var product = CreateProduct();

        product.ChangeManufacturePhone("09351234567");

        Assert.Equal("09351234567", product.ManufacturePhone.Value);
    }

    [Fact]
    public void ChangeManufacturePhone_InvalidPhone_ThrowsAndKeepsPreviousPhone()
    {
        var product = CreateProduct();

        Assert.Throws<ManufacturePhoneLengthIsInvalidException>(
            () => product.ChangeManufacturePhone("123"));
        Assert.Equal(ValidPhone, product.ManufacturePhone.Value);
    }

    [Fact]
    public void ChangeManufactureEmail_MixedCaseEmail_NormalizesEmail()
    {
        var product = CreateProduct();

        product.ChangeManufactureEmail("  Support@NadinShop.COM  ");

        Assert.Equal("support@nadinshop.com", product.ManufactureEmail.Value);
    }

    [Fact]
    public void ChangeManufactureEmail_InvalidEmail_ThrowsAndKeepsPreviousEmail()
    {
        var product = CreateProduct();

        Assert.Throws<ManufactureEmailFormatIsInvalidException>(
            () => product.ChangeManufactureEmail("plainaddress"));
        Assert.Equal("test@example.com", product.ManufactureEmail.Value);
    }

    [Fact]
    public void ChangeManufactureContact_ValidValues_UpdatesBothValues()
    {
        var product = CreateProduct();

        product.ChangeManufactureContact("09351234567", "Support@NadinShop.COM");

        Assert.Equal("09351234567", product.ManufacturePhone.Value);
        Assert.Equal("support@nadinshop.com", product.ManufactureEmail.Value);
    }

    [Fact]
    public void ChangeManufactureContact_InvalidPhone_ThrowsAndKeepsPreviousValues()
    {
        var product = CreateProduct();

        Assert.Throws<ManufacturePhoneLengthIsInvalidException>(
            () => product.ChangeManufactureContact("123", "support@nadinshop.com"));

        Assert.Equal(ValidPhone, product.ManufacturePhone.Value);
        Assert.Equal("test@example.com", product.ManufactureEmail.Value);
    }

    [Fact]
    public void ChangeManufactureContact_InvalidEmail_ThrowsAndKeepsPreviousValues()
    {
        var product = CreateProduct();

        Assert.Throws<ManufactureEmailFormatIsInvalidException>(
            () => product.ChangeManufactureContact("09351234567", "plainaddress"));

        Assert.Equal(ValidPhone, product.ManufacturePhone.Value);
        Assert.Equal("test@example.com", product.ManufactureEmail.Value);
    }

    [Fact]
    public void MarkAsAvailable_UnavailableProduct_SetsIsAvailableTrue()
    {
        var product = CreateProduct(isAvailable: false);

        product.MarkAsAvailable();

        Assert.True(product.IsAvailable);
    }

    [Fact]
    public void MarkAsAvailable_AlreadyAvailableProduct_KeepsIsAvailableTrue()
    {
        var product = CreateProduct(isAvailable: true);

        product.MarkAsAvailable();

        Assert.True(product.IsAvailable);
    }

    [Fact]
    public void MarkAsUnavailable_AvailableProduct_SetsIsAvailableFalse()
    {
        var product = CreateProduct(isAvailable: true);

        product.MarkAsUnavailable();

        Assert.False(product.IsAvailable);
    }

    [Fact]
    public void MarkAsUnavailable_AlreadyUnavailableProduct_KeepsIsAvailableFalse()
    {
        var product = CreateProduct(isAvailable: false);

        product.MarkAsUnavailable();

        Assert.False(product.IsAvailable);
    }

    [Fact]
    public void Equals_DifferentIds_ReturnsFalse()
    {
        var first = Product.Create(ValidName, ValidPhone, ValidEmail, ValidProduceDate, isAvailable: true);
        var second = Product.Create(ValidName, ValidPhone, ValidEmail, ValidProduceDate, isAvailable: true);

        Assert.NotEqual(first, second);
        Assert.True(first != second);
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        var product = CreateProduct();
        var sameReference = product;

        Assert.Equal(product, sameReference);
        Assert.True(product == sameReference);
        Assert.Equal(product.GetHashCode(), sameReference.GetHashCode());
    }
}
