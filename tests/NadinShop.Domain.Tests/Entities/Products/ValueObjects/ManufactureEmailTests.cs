using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Domain.Tests.Entities.Products.ValueObjects;

public class ManufactureEmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@example.co.uk")]
    [InlineData("user@sub.domain.com")]
    [InlineData("a@b.co")]
    [InlineData("first.last@example.com")]
    public void Create_ValidEmail_ReturnsEmail(string input)
    {
        var email = ManufactureEmail.Create(input);

        Assert.Equal(input, email.Value);
    }

    [Fact]
    public void Create_MixedCaseEmail_NormalizesToLowercase()
    {
        var email = ManufactureEmail.Create("Test.User@Example.COM");

        Assert.Equal("test.user@example.com", email.Value);
    }

    [Fact]
    public void Create_ValueWithSurroundingWhitespace_TrimsWhitespace()
    {
        var email = ManufactureEmail.Create("  test@example.com  ");

        Assert.Equal("test@example.com", email.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyValue_ThrowsManufactureEmailCannotBeEmptyException(string? value)
    {
        Assert.Throws<ManufactureEmailCannotBeEmptyException>(() => ManufactureEmail.Create(value));
    }

    [Fact]
    public void Create_MaximumLengthEmail_ReturnsEmail()
    {
        var localPart = new string('a', 64);
        var domain = new string('b', 63) + "." + new string('c', 63) + "." + new string('d', 61);
        var value = localPart + "@" + domain;

        Assert.Equal(ManufactureEmail.MaxLength, value.Length);

        var email = ManufactureEmail.Create(value);

        Assert.Equal(value, email.Value);
    }

    [Fact]
    public void Create_EmailLongerThanMaximumLength_ThrowsManufactureEmailLengthIsInvalidException()
    {
        var localPart = new string('a', 64);
        var domain = new string('b', 63) + "." + new string('c', 63) + "." + new string('d', 62);
        var value = localPart + "@" + domain;

        Assert.Throws<ManufactureEmailLengthIsInvalidException>(() => ManufactureEmail.Create(value));
    }

    [Fact]
    public void Create_EmailShorterThanMinimumLength_ThrowsManufactureEmailLengthIsInvalidException()
    {
        Assert.Throws<ManufactureEmailLengthIsInvalidException>(() => ManufactureEmail.Create("a@b"));
    }

    [Fact]
    public void Create_LocalPartLongerThan64Characters_ThrowsManufactureEmailFormatIsInvalidException()
    {
        var value = new string('a', 65) + "@example.com";

        Assert.Throws<ManufactureEmailFormatIsInvalidException>(() => ManufactureEmail.Create(value));
    }

    [Theory]
    [InlineData("plainaddress")]
    [InlineData("user@")]
    [InlineData("@example.com")]
    [InlineData("user@example")]
    [InlineData("user name@example.com")]
    [InlineData("user@exa mple.com")]
    [InlineData("user@@example.com")]
    [InlineData(".user@example.com")]
    [InlineData("user.@example.com")]
    [InlineData("user..name@example.com")]
    [InlineData("user@-example.com")]
    [InlineData("user@example..com")]
    [InlineData("user@example.c")]
    [InlineData("user@example.123")]
    public void Create_InvalidFormat_ThrowsManufactureEmailFormatIsInvalidException(string value)
    {
        Assert.Throws<ManufactureEmailFormatIsInvalidException>(() => ManufactureEmail.Create(value));
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var first = ManufactureEmail.Create("Test@Example.com");
        var second = ManufactureEmail.Create("test@example.com");

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var first = ManufactureEmail.Create("first@example.com");
        var second = ManufactureEmail.Create("second@example.com");

        Assert.NotEqual(first, second);
    }
}
