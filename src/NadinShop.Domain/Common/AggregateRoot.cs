namespace NadinShop.Domain.Common;

public class AggregateRoot<TId>(TId id) : Entity<TId>(id)
where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void Clear() => _domainEvents.Clear();
}
