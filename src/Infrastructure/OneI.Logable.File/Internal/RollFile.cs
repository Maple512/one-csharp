namespace OneI.Logable;

using System;

internal readonly struct RollFile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RollFile"/> class.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <param name="dateTime">The date time.</param>
    /// <param name="sequenceNumber">The sequence number.</param>
    public RollFile(string filename, DateTime? dateTime, int? sequenceNumber)
    {
        Filename = filename;
        DateTime = dateTime;
        SequenceNumber = sequenceNumber;
    }

    /// <summary>
    /// Gets the filename.
    /// </summary>
    public string Filename { get; }

    /// <summary>
    /// Gets the date time.
    /// </summary>
    public DateTime? DateTime { get; }

    /// <summary>
    /// Gets the sequence number.
    /// </summary>
    public int? SequenceNumber { get; }
}
