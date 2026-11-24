using NadinShop.Domain.Common;

namespace NadinShop.Domain.Tests.Common;

public class ValueObjectTests
{
    [Fact]
    public void Equals_SameTypeAndValue_ReturnsTrue()
    {
        string productName = "Product Name";
        var first = new TestValueObject(productName);
        var second = new TestValueObject(productName);

        Assert.Equal(first,second);
        Assert.Equal(first.GetHashCode(),second.GetHashCode());
    }
    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
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
