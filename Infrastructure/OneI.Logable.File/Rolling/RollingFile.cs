namespace OneI.Logable.Rolling;

using System;

public readonly struct RollingFile
{
    public RollingFile(string name, DateTime? dateTime, int? sequenceNumber)
    {
        Name = name;
        DateTime = dateTime;
        SequenceNumber = sequenceNumber;
    }

    public string Name { get; }

    public DateTime? DateTime { get; }

    public int? SequenceNumber { get; }
}
