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
    public void Create_accepts_valid_emails(string input)
    {
        var email = ManufactureEmail.Create(input);

        Assert.Equal(input, email.Value);
    }

    [Fact]
    public void Create_normalizes_email_to_lowercase()
    {
        var email = ManufactureEmail.Create("Test.User@Example.COM");

        Assert.Equal("test.user@example.com", email.Value);
    }

    [Fact]
    public void Create_trims_surrounding_whitespace()
    {
        var email = ManufactureEmail.Create("  test@example.com  ");

        Assert.Equal("test@example.com", email.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_throws_when_value_is_empty(string? value)
    {
        Assert.Throws<ManufactureEmailCannotBeEmptyException>(() => ManufactureEmail.Create(value));
    }

    [Fact]
    public void Create_accepts_email_with_maximum_length()
    {
        var localPart = new string('a', 64);
        var domain = new string('b', 63) + "." + new string('c', 63) + "." + new string('d', 61);
        var value = localPart + "@" + domain;

        Assert.Equal(ManufactureEmail.MaxLength, value.Length);

        var email = ManufactureEmail.Create(value);

        Assert.Equal(value, email.Value);
    }

    [Fact]
    public void Create_throws_when_email_is_longer_than_maximum_length()
    {
        var localPart = new string('a', 64);
        var domain = new string('b', 63) + "." + new string('c', 63) + "." + new string('d', 62);
        var value = localPart + "@" + domain;

        Assert.Throws<ManufactureEmailLengthIsInvalidException>(() => ManufactureEmail.Create(value));
    }

    [Fact]
    public void Create_throws_when_email_is_shorter_than_minimum_length()
    {
        Assert.Throws<ManufactureEmailLengthIsInvalidException>(() => ManufactureEmail.Create("a@b"));
    }

    [Fact]
    public void Create_throws_when_local_part_is_longer_than_64_characters()
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
    public void Create_throws_when_format_is_invalid(string value)
    {
        Assert.Throws<ManufactureEmailFormatIsInvalidException>(() => ManufactureEmail.Create(value));
    }

    [Fact]
    public void ManufactureEmails_with_same_value_are_equal()
    {
        var first = ManufactureEmail.Create("Test@Example.com");
        var second = ManufactureEmail.Create("test@example.com");

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ManufactureEmails_with_different_values_are_not_equal()
    {
        var first = ManufactureEmail.Create("first@example.com");
        var second = ManufactureEmail.Create("second@example.com");

        Assert.NotEqual(first, second);
    }
}
