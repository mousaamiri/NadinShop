using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Domain.Tests.Entities.Products.ValueObjects;

public class ManufacturePhoneTests
{
    [Theory]
    [InlineData("09121234567")]
    [InlineData("+989121234567")]
    [InlineData("0912 123 4567")]
    [InlineData("0912-123-4567")]
    [InlineData("0912(123)4567")]
    [InlineData("1234567")]
    [InlineData("123456789012345")]
    public void Create_ValidPhoneNumber_ReturnsPhone(string input)
    {
        var phone = ManufacturePhone.Create(input);

        Assert.Equal(input, phone.Value);
    }

    [Fact]
    public void Create_ValueWithSurroundingWhitespace_TrimsWhitespace()
    {
        var phone = ManufacturePhone.Create("  09121234567  ");

        Assert.Equal("09121234567", phone.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyValue_ThrowsManufacturePhoneCannotBeEmptyException(string? value)
    {
        Assert.Throws<ManufacturePhoneCannotBeEmptyException>(() => ManufacturePhone.Create(value));
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("1234567890123456")]
    [InlineData("0912 12")]
    public void Create_DigitCountOutOfRange_ThrowsManufacturePhoneLengthIsInvalidException(string value)
    {
        Assert.Throws<ManufacturePhoneLengthIsInvalidException>(() => ManufacturePhone.Create(value));
    }

    [Theory]
    [InlineData("abc1234567")]
    [InlineData("0912abc4567")]
    [InlineData("0912+1234567")]
    [InlineData("0912/1234567")]
    [InlineData("(0912)1234567")]
    public void Create_InvalidFormat_ThrowsManufacturePhoneFormatIsInvalidException(string value)
    {
        Assert.Throws<ManufacturePhoneFormatIsInvalidException>(() => ManufacturePhone.Create(value));
    }

    [Fact]
    public void Create_ValueLongerThanStoredLength_ThrowsManufacturePhoneIsTooLongException()
    {
        var value = new string('1', ManufacturePhone.MaxStoredLength + 1);

        Assert.Throws<ManufacturePhoneIsTooLongException>(() => ManufacturePhone.Create(value));
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var first = ManufacturePhone.Create("09121234567");
        var second = ManufacturePhone.Create(" 09121234567 ");

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var first = ManufacturePhone.Create("09121234567");
        var second = ManufacturePhone.Create("09121234568");

        Assert.NotEqual(first, second);
    }
}
