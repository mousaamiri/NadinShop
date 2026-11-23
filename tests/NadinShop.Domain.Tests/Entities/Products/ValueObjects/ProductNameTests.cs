using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Domain.Tests.Entities.Products.ValueObjects;

public class ProductNameTests
{
    [Fact]
    public void ProductName_stores_the_given_value()
    {
        const string value = "Samsung Galaxy S24 phone";

        var productName = new ProductName(value);

        Assert.Equal(value, productName.Value);
    }

    [Fact]
    public void ProductNames_with_same_value_are_equal()
    {
        const string value = "Samsung phone";
        var first = new ProductName(value);
        var second = new ProductName(value);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ProductNames_with_different_values_are_not_equal()
    {
        var first = new ProductName("Samsung phone");
        var second = new ProductName("Apple phone");

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void ProductName_equality_is_case_sensitive()
    {
        var first = new ProductName("Galaxy S24");
        var second = new ProductName("galaxy s24");

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void ProductName_is_not_equal_to_null()
    {
        var productName = new ProductName("Samsung phone");

        Assert.False(productName.Equals(null));
    }
}
