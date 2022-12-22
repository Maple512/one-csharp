namespace OneI.Domainable;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
/// <summary>
/// The entity.
/// </summary>

public interface IEntity
{
    /// <summary>
    /// Gets the keys.
    /// </summary>
    /// <returns>A list of object.</returns>
    IEnumerable<object> GetKeys();
}
/// <summary>
/// The entity.
/// </summary>

public interface IEntity<TKey> : IEntity
    where TKey : notnull
{
    /// <summary>
    /// Gets the id.
    /// </summary>
    TKey Id { get; }
}
/// <summary>
/// The entity.
/// </summary>

[Serializable]
public abstract class Entity : IEntity, IEqualityComparer<Entity>, IEquatable<Entity>
{
    /// <summary>
    /// Gets the keys.
    /// </summary>
    /// <returns>A list of object.</returns>
    public abstract IEnumerable<object> GetKeys();

    /// <summary>
    /// Equals the.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns>A bool.</returns>
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

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns>An int.</returns>
    public int GetHashCode([DisallowNull] Entity obj)
    {
        return obj.GetKeys().GetHashCode();
    }

    /// <summary>
    /// Equals the.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns>A bool.</returns>
    public bool Equals(Entity? other)
    {
        return other != null && GetKeys().SequenceEqual(other.GetKeys());
    }

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        return $"[Entity: {GetType().Name}, Keys: {string.Join(',', GetKeys())}]";
    }

    /// <summary>
    /// Equals the.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns>A bool.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Entity);
    }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>An int.</returns>
    public override int GetHashCode()
    {
        return GetKeys().GetHashCode();
    }
}
/// <summary>
/// The entity.
/// </summary>

[Serializable]
public abstract class Entity<TKey> : Entity, IEntity<TKey>, IEqualityComparer<Entity<TKey>>, IEquatable<Entity<TKey>>
    where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    protected Entity()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    /// <param name="id">The id.</param>
    protected Entity(TKey id) => Id = id;

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [Key]
    public virtual TKey Id { get; protected set; }

    /// <summary>
    /// Equals the.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns>A bool.</returns>
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

    /// <summary>
    /// Gets the keys.
    /// </summary>
    /// <returns>A list of object.</returns>
    public sealed override IEnumerable<object> GetKeys()
    {
        yield return Id;
    }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns>An int.</returns>
    public int GetHashCode([DisallowNull] Entity<TKey> obj)
    {
        return obj.Id.GetHashCode();
    }

    /// <summary>
    /// Equals the.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns>A bool.</returns>
    public bool Equals(Entity<TKey>? other)
    {
        return other != null && other.Id.Equals(Id);
    }

    /// <summary>
    /// Equals the.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns>A bool.</returns>
    public override bool Equals([AllowNull] object obj)
    {
        return Equals(obj as Entity<TKey>);
    }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>An int.</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
