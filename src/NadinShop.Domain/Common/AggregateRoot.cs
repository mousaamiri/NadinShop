namespace NadinShop.Domain.Common;

public class AggregateRoot<TId>: Entity<TId>
where TId : notnull
{
    //Domain event list
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;
    //Constructor
    protected AggregateRoot(){}//For Ef
    protected AggregateRoot(TId id):base(id){}
    //methods 
    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void Clear() => _domainEvents.Clear();
}
