using NadinShop.Domain.Common;

namespace NadinShop.Domain.Tests.Common;

public class EntityTests
{
    [Fact]
    public void Entities_of_same_type_with_same_id_are_equal()
    {

        var id = Guid.NewGuid();
        var firest = new TestEntity(id);
        var second = new TestEntity(id);

        Assert.Equal(firest,second);
        Assert.True(firest==second);
        Assert.False(firest!=second);
        Assert.Equal(firest.GetHashCode(),second.GetHashCode());
    }

    [Fact]
    public void Entities_with_different_ids_are_not_equals()
    {
        var firest = new TestEntity(Guid.NewGuid());
        var second = new TestEntity(Guid.NewGuid());

        Assert.NotEqual(firest, second);
        Assert.False(firest == second);
        Assert.True(firest != second);
        Assert.NotEqual(firest.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void AggregateRoot_tracks_and_clear_domain_events()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        var domainEvent = new TestDomainEvent(DateTimeOffset.Now);

        aggregate.RaiseEvent(domainEvent);

        Assert.Single(aggregate.DomainEvents);
        Assert.Contains(domainEvent,aggregate.DomainEvents);

        aggregate.Clear();
        Assert.Empty(aggregate.DomainEvents);

    }
    private sealed class TestEntity(Guid id) : Entity<Guid>(id);

    private sealed class TestAggregate(Guid id) : AggregateRoot<Guid>(id)
    {
        public void RaiseEvent(IDomainEvent domainEvent)
        {
            Raise(domainEvent);
        }
    }

    private sealed record TestDomainEvent(DateTimeOffset OccurredOn) : IDomainEvent;
}
