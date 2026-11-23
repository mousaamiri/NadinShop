using System.Diagnostics;
using System.Security.AccessControl;

namespace NadinShop.Domain.Common;

public abstract class Entity<TId>(TId id) : IEquatable<Entity<TId>>
    where TId : notnull
{

    public TId Id { get; protected set; } = id;
    public static bool operator == (Entity<TId>? left, Entity<TId>? right){
        return Equals(left, right);
    }

    public static bool operator != (Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }

    public bool Equals(Entity<TId>? other)
    {
        return other is not null
               && GetType() == other.GetType()
               && EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }
}
