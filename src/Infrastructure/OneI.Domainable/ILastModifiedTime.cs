namespace OneI.Domainable;

using System;
/// <summary>
/// The last modified.
/// </summary>

public interface ILastModified
{
    /// <summary>
    /// Gets the last modified time.
    /// </summary>
    DateTimeOffset LastModifiedTime { get; }
}
