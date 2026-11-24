using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Domain.Tests.Entities.Products.ValueObjects;

public class ProductNameTests
{
    [Fact]
    public void Constructor_ValidValue_StoresValue()
    {
        const string value = "Samsung Galaxy S24 phone";

        var productName = new ProductName(value);

        Assert.Equal(value, productName.Value);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        const string value = "Samsung phone";
        var first = new ProductName(value);
        var second = new ProductName(value);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var first = new ProductName("Samsung phone");
        var second = new ProductName("Apple phone");

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equals_DifferentCasing_ReturnsFalse()
    {
        var first = new ProductName("Galaxy S24");
        var second = new ProductName("galaxy s24");

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var productName = new ProductName("Samsung phone");

        Assert.False(productName.Equals(null));
    }
}
