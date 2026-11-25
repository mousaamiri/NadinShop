using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Domain.Tests.Entities.Products.ValueObjects;

public class ProduceDateTests
{
    [Fact]
    public void Create_ValidDate_StoresCalendarDay()
    {
        var date = new DateTime(2024, 5, 20, 10, 30, 0, DateTimeKind.Utc);

        var produceDate = ProduceDate.Create(date);

        Assert.Equal(date.Date, produceDate.Value);
    }

    [Fact]
    public void Create_DateWithTime_TruncatesTimePart()
    {
        var produceDate = ProduceDate.Create(new DateTime(2024, 5, 20, 23, 59, 59, DateTimeKind.Utc));

        Assert.Equal(TimeSpan.Zero, produceDate.Value.TimeOfDay);
    }

    [Fact]
    public void Create_DefaultDate_ThrowsProduceDateCannotBeEmptyException()
    {
        Assert.Throws<ProduceDateCannotBeEmptyException>(() => ProduceDate.Create(default));
    }

    [Fact]
    public void Create_MinValueDate_ThrowsProduceDateCannotBeEmptyException()
    {
        Assert.Throws<ProduceDateCannotBeEmptyException>(() => ProduceDate.Create(DateTime.MinValue));
    }

    [Fact]
    public void Create_FutureDate_ThrowsProduceDateCannotBeInFutureException()
    {
        var futureDate = DateTime.UtcNow.AddDays(1);

        Assert.Throws<ProduceDateCannotBeInFutureException>(() => ProduceDate.Create(futureDate));
    }

    [Fact]
    public void Equals_SameDate_ReturnsTrue()
    {
        var date = new DateTime(2024, 5, 20, 0, 0, 0, DateTimeKind.Utc);
        var first = ProduceDate.Create(date);
        var second = ProduceDate.Create(date);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentDates_ReturnsFalse()
    {
        var first = ProduceDate.Create(new DateTime(2024, 5, 20, 0, 0, 0, DateTimeKind.Utc));
        var second = ProduceDate.Create(new DateTime(2024, 5, 21, 0, 0, 0, DateTimeKind.Utc));

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void ToString_ValidDate_ReturnsIsoDate()
    {
        var produceDate = ProduceDate.Create(new DateTime(2024, 5, 20, 13, 45, 0, DateTimeKind.Utc));

        Assert.Equal("2024-05-20", produceDate.ToString());
    }

}
