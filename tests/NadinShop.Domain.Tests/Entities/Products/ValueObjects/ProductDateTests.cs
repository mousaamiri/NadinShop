using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Domain.Tests.Entities.Products.ValueObjects;

public class ProductDateTests
{
    [Fact]
    public void Create_stores_the_given_date()
    {
        var date = new DateTime(2024, 5, 20, 10, 30, 0, DateTimeKind.Utc);

        var produceDate = ProduceDate.Create(date);

        Assert.Equal(date, produceDate.Value);
    }

    [Fact]
    public void Create_throws_when_date_is_default()
    {
        Assert.Throws<ProduceDateCannotBeEmptyException>(() => ProduceDate.Create(default));
    }

    [Fact]
    public void Create_throws_when_date_is_min_value()
    {
        Assert.Throws<ProduceDateCannotBeEmptyException>(() => ProduceDate.Create(DateTime.MinValue));
    }

    [Fact]
    public void Create_throws_when_date_is_in_the_future()
    {
        var futureDate = DateTime.UtcNow.AddDays(1);

        Assert.Throws<ProduceDateCannotBeInFutureException>(() => ProduceDate.Create(futureDate));
    }

    [Fact]
    public void ProduceDates_with_same_date_are_equal()
    {
        var date = new DateTime(2024, 5, 20, 0, 0, 0, DateTimeKind.Utc);
        var first = ProduceDate.Create(date);
        var second = ProduceDate.Create(date);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ProduceDates_with_different_dates_are_not_equal()
    {
        var first = ProduceDate.Create(new DateTime(2024, 5, 20, 0, 0, 0, DateTimeKind.Utc));
        var second = ProduceDate.Create(new DateTime(2024, 5, 21, 0, 0, 0, DateTimeKind.Utc));

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void ToString_returns_date_in_iso_format()
    {
        var produceDate = ProduceDate.Create(new DateTime(2024, 5, 20, 13, 45, 0, DateTimeKind.Utc));

        Assert.Equal("2024-05-20", produceDate.ToString());
    }

}
