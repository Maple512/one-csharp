namespace OneI.Domainable;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public interface IEntity
{
    IEnumerable<object> GetKeys();
}

public interface IEntity<TKey> : IEntity
    where TKey : notnull
{
    TKey Id { get; }
}

[Serializable]
public abstract class Entity : IEntity, IEqualityComparer<Entity>, IEquatable<Entity>
{
    public abstract IEnumerable<object> GetKeys();

    public bool Equals(Entity? x, Entity? y)
    {
        if(x == null && y == null)
        {
            return true;
        }

        if(x == null || y == null)
        {
            return false;
        }

        return x.GetKeys().SequenceEqual(y.GetKeys());
    }

    public int GetHashCode([DisallowNull] Entity obj) => obj.GetKeys().GetHashCode();

    public bool Equals(Entity? other) => other != null && GetKeys().SequenceEqual(other.GetKeys());

    public override string ToString() => $"[Entity: {GetType().Name}, Keys: {string.Join(',', GetKeys())}]";

    public override bool Equals(object? obj) => Equals(obj as Entity);

    public override int GetHashCode() => GetKeys().GetHashCode();
}

[Serializable]
public abstract class Entity<TKey> : Entity, IEntity<TKey>, IEqualityComparer<Entity<TKey>>, IEquatable<Entity<TKey>>
    where TKey : notnull
{
    protected Entity() { }

    protected Entity(TKey id) => Id = id;

    [Key]
    public virtual TKey Id { get; protected set; }

    public bool Equals(Entity<TKey>? x, Entity<TKey>? y)
    {
        if(x == null && y == null)
        {
            return true;
        }

        if(x == null || y == null)
        {
            return false;
        }

        return x.Id.Equals(y.Id);
    }

    public sealed override IEnumerable<object> GetKeys()
    {
        yield return Id;
    }

    public int GetHashCode([DisallowNull] Entity<TKey> obj) => obj.Id.GetHashCode();

    public bool Equals(Entity<TKey>? other) => other != null && other.Id.Equals(Id);

    public override bool Equals([AllowNull] object obj) => Equals(obj as Entity<TKey>);

    public override int GetHashCode() => Id.GetHashCode();
}
