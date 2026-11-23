namespace NadinShop.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
