namespace OneI.Logable.Rolling;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

public class PathRoller
{
    private const string PeriodMatchGroup = "period",
         SequenceNumberMatchGroup = "sequence";

    private readonly string _directory;
    private readonly string _filenamePrefix;
    private readonly string _filenameSuffix;
    private readonly Regex _filenameMatcher;
    private readonly RollingPolicy _policy;
    private readonly string _periodFormat;

    public PathRoller(string path, RollingPolicy policy)
    {
        CheckTools.NotNull(path);

        _policy = policy;
        _periodFormat = policy.GetFormatString();

        var pathDirectory = Path.GetDirectoryName(path);
        if(pathDirectory.IsNullOrEmpty())
        {
            pathDirectory = Directory.GetCurrentDirectory();
        }

        _directory = pathDirectory;
        _filenamePrefix = Path.GetFileNameWithoutExtension(path);
        _filenameSuffix = Path.GetExtension(path);
        _filenameMatcher = new Regex(
                "^" +
                Regex.Escape(_filenamePrefix) +
                "(?<" + PeriodMatchGroup + ">\\d{" + _periodFormat.Length + "})" +
                "(?<" + SequenceNumberMatchGroup + ">_[0-9]{3,}){0,1}" +
                Regex.Escape(_filenameSuffix) +
                "$",
                RegexOptions.Compiled);

        DirectorySearchPattern = $"{_filenamePrefix}*{_filenameSuffix}";
    }

    public string FileDirectory => _directory;

    public string DirectorySearchPattern { get; }

    public void GetFilePath(DateTime datetime, int? sequenceNumber, out string path)
    {
        var currentCheckPoint = GetCurrentCheckpoint(datetime);

        var tok = currentCheckPoint?.ToString(_periodFormat)
            ?? string.Empty;

        if(sequenceNumber.HasValue)
        {
            tok += $"_{sequenceNumber.Value:000}";
        }

        path = Path.Combine(_directory, $"{_filenamePrefix}{tok}{_filenameSuffix}");
    }

    public IEnumerable<RollingFile> SelectMatches(IEnumerable<string> files)
    {
        foreach(var file in files)
        {
            var match = _filenameMatcher.Match(file);
            if(match.Success)
            {
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
                    var datetimePart = periodGroup.Captures[0].Value;
                    if(DateTime.TryParseExact(
                        datetimePart,
                        _periodFormat,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var datetime))
                    {
                        period = datetime;
                    }
                }

                yield return new RollingFile(file, period, inc);
            }
        }
    }

    public DateTime? GetCurrentCheckpoint(DateTime instant) => _policy.GetCurrentCheckPoint(instant);

    public DateTime? GetNextCheckpoint(DateTime instant) => _policy.GetNextCheckpoint(instant);
}
