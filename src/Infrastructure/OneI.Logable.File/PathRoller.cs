namespace OneI.Logable;

using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
/// <summary>
/// The path roller.
/// </summary>

internal class PathRoller
{
    /// <summary>
    /// 周期
    /// </summary>
    private const string PeriodMatchGroup = "period";

    /// <summary>
    /// 序列号
    /// </summary>
    private const string SequenceNumberMatchGroup = "sequence";
    private readonly string _fileNamePrefix;
    private readonly string _fileNameSuffix;
    private readonly Regex _fileNameMatcher;
    private readonly RollFrequency _rollFrequency;
    private readonly string _periodFormat;

    /// <summary>
    /// Initializes a new instance of the <see cref="PathRoller"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="rollFrequency">The roll frequency.</param>
    public PathRoller(string path, RollFrequency rollFrequency)
    {
        Check.NotNullOrWhiteSpace(path);

        _rollFrequency = rollFrequency;
        _periodFormat = rollFrequency.GetFormat();

        var directory = Path.GetDirectoryName(path);
        if(directory.IsNullOrWhiteSpace())
        {
            directory = Directory.GetCurrentDirectory();
        }

        if(Directory.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }

        PathDirectory = Path.GetFullPath(directory);

        _fileNamePrefix = Path.GetFileNameWithoutExtension(path);
        _fileNameSuffix = Path.GetExtension(path);
        _fileNameMatcher = new Regex(
            $"^" +
            $"{Regex.Escape(_fileNamePrefix)}" +
            $"(?<{PeriodMatchGroup}>\\d{{{_periodFormat.Length}}})" +
            $"(?<{SequenceNumberMatchGroup}>_[0-9]{{3,}}){{0,1}}" +
            $"{Regex.Escape(_fileNameSuffix)}" +
            $"$", RegexOptions.Compiled);

        DirectorySearchPattern = $"{_fileNamePrefix}*{_fileNameSuffix}";
    }

    /// <summary>
    /// Gets the path directory.
    /// </summary>
    public string PathDirectory { get; }

    /// <summary>
    /// Gets the directory search pattern.
    /// </summary>
    public string DirectorySearchPattern { get; }

    /// <summary>
    /// Gets the log file path.
    /// </summary>
    /// <param name="datetime">The datetime.</param>
    /// <param name="sequenceNumber">The sequence number.</param>
    /// <returns>A string.</returns>
    public string GetLogFilePath(DateTime datetime, int? sequenceNumber)
    {
        var currentPeriod = GetCurrentPeriod(datetime);

        var tok = currentPeriod?.ToString(_periodFormat) ?? string.Empty;

        if(sequenceNumber.HasValue)
        {
            tok += "_" + sequenceNumber.Value.ToString("000");
        }

        return Path.Combine(PathDirectory, _fileNamePrefix + tok + _fileNameSuffix);
    }

    /// <summary>
    /// Selects the matches.
    /// </summary>
    /// <param name="filenames">The filenames.</param>
    /// <returns>A list of RollFiles.</returns>
    public IEnumerable<RollFile> SelectMatches(IEnumerable<string> filenames)
    {
        foreach(var filename in filenames)
        {
            var match = _fileNameMatcher.Match(filename);
            if(!match.Success)
            {
                continue;
            }

            int? inc = null;
            var incGroup = match.Groups[SequenceNumberMatchGroup];
            if(incGroup.Captures.Count != 0)
            {
                var incPart = incGroup.Captures[0].Value[1..];

                inc = int.Parse(incPart);
            }

            DateTime? period = null;
            var periodGroup = match.Groups[PeriodMatchGroup];
            if(periodGroup.Captures.Count != 0)
            {
                var dateTimePart = periodGroup.Captures[0].Value;
                if(DateTime.TryParseExact(
                    dateTimePart,
                    _periodFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dateTime))
                {
                    period = dateTime;
                }
            }

            yield return new(filename, period, inc);
        }
    }

    /// <summary>
    /// Gets the current period.
    /// </summary>
    /// <param name="datetime">The datetime.</param>
    /// <returns>A DateTime? .</returns>
    public DateTime? GetCurrentPeriod(DateTime datetime)
    {
        return _rollFrequency.GetCurrentPeriod(datetime);
    }

    /// <summary>
    /// Gets the next period.
    /// </summary>
    /// <param name="datetime">The datetime.</param>
    /// <returns>A DateTime? .</returns>
    public DateTime? GetNextPeriod(DateTime datetime)
    {
        return _rollFrequency.GetNextPeriod(datetime);
    }
}
