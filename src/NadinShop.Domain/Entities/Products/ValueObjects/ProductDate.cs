using NadinShop.Domain.Common;

namespace NadinShop.Domain.Entities.Products.ValueObjects;

public sealed class ProduceDate:ValueObject<DateTime>
{
    public DateTime Value { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    //Constructor
    private ProduceDate(DateTime value):base(value)
    {
        Value = value;
    }

    //Factory method
    public static ProduceDate Create(DateTime value)
    {
        if (value == default || value == DateTime.MinValue)
            throw new ProduceDateCannotBeEmptyException();

        // The production date must not be in the future.
        if (value > DateTime.UtcNow)
            throw new ProduceDateCannotBeInFutureException();

        // Stored as a calendar day: the database column is a date, so the time part is dropped.
        return new ProduceDate(value.Date);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");

    public static implicit operator DateTime(ProduceDate produceDate) => produceDate.Value;
}

public sealed class ProduceDateCannotBeEmptyException()
    : DomainException("The produce date cannot be empty.");

public sealed class ProduceDateCannotBeInFutureException()
    : DomainException("The produce date cannot be in the future.");
