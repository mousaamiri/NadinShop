namespace NadinShop.Domain.Common;

public abstract class ValueObject<TValueType>(TValueType value):IEquatable<ValueObject<TValueType>>
{

    //Properties
    public TValueType Value { get; protected set; } = value;
    protected abstract IEnumerable<object> GetEqualityComponents();
    public bool Equals(ValueObject<TValueType>? other)
    {
        if (other is null)
            return false;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        return obj.GetType() == GetType() && Equals((ValueObject<TValueType>)obj);
    }

    public override int GetHashCode()
    {
        //Generating a unique and comparable integer (int) based on all the fields that make up the object.
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) =>
                current * 23 + (obj?.GetHashCode() ?? 0));
    }

}
