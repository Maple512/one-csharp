namespace OneI.Domainable;

using System;
/// <summary>
/// The created time.
/// </summary>

public interface ICreatedTime
{
    /// <summary>
    /// Gets the created time.
    /// </summary>
    DateTimeOffset CreatedTime { get; }
}
