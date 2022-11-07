namespace OneI.Domainable;

using System;

public interface ILastModified
{
    DateTimeOffset LastModifiedTime { get; }
}
