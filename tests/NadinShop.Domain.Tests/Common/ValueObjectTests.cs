using NadinShop.Domain.Common;

namespace NadinShop.Domain.Tests.Common;

public class ValueObjectTests
{
    [Fact]
    public void ValueObjects_of_same_type_and_same_value_are_equals()
    {
        string productName = "Product Name";
        var first = new TestValueObject(productName);
        var second = new TestValueObject(productName);

        Assert.Equal(first,second);
        Assert.Equal(first.GetHashCode(),second.GetHashCode());
    }
    [Fact]
    public void ValueObjects_with_different_value_are_not_equals()
    {
        var first = new TestValueObject("Product 1");
        var second = new TestValueObject("Product 2");

        Assert.NotEqual(first, second);
        Assert.NotEqual(first.GetHashCode(), second.GetHashCode());
    }

    private class TestValueObject(string value) : ValueObject<string>(value)
    {
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
