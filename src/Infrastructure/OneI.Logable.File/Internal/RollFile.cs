namespace OneI.Logable;

using System;

internal readonly struct RollFile
{
    public RollFile(string filename, DateTime? dateTime, int? sequenceNumber)
    {
        Filename = filename;
        DateTime = dateTime;
        SequenceNumber = sequenceNumber;
    }

    public string Filename { get; }

    public DateTime? DateTime { get; }

    public int? SequenceNumber { get; }
}
