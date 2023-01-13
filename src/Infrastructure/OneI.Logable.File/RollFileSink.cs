namespace OneI.Logable;

using OneI.Logable;
using OneI.Logable.Rendering;

internal class RollFileSink : ILoggerSink, IFileFlusher, IDisposable
{
    private static readonly object _lock = new();

    private readonly PathRoller _roller;
    private readonly long? _fileSizeMaxBytes;
    private readonly int? _retainedFileCountMax;
    private readonly TimeSpan? _retainedFileTimeMax;
    private readonly Encoding? _encoding;
    private readonly bool _buffered;
    private readonly bool _shared;
    private bool _disposed;
    private DateTime? _nextPeroid;
    private IFileSink? _sink;
    private int? _currentSequence;
    private readonly ILoggerRenderer _renderer;

    public RollFileSink(
        string path,
        RollFrequency frequency,
        ILoggerRenderer provider,
        long? fileSizeMaxBytes,
        int? retainedFileCountMax,
        TimeSpan? retainedFileTimeMax,
        Encoding? encoding,
        bool buffered,
        bool shared)
    {
        _renderer= provider;
        _roller = new PathRoller(path.ToString(), frequency);
        _fileSizeMaxBytes = fileSizeMaxBytes;
        _retainedFileCountMax = retainedFileCountMax;
        _retainedFileTimeMax = retainedFileTimeMax;
        _encoding = encoding ?? Encoding.UTF8;
        _buffered = buffered;
        _shared = shared;
    }

    /// <summary>
    /// Flushes the to disk.
    /// </summary>
    public void FlushToDisk()
    {
        lock(_lock)
        {
            _sink?.FlushToDisk();
        }
    }

    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    public  void Invoke(in LoggerContext context)
    {
        if(_disposed)
        {
            throw new ObjectDisposedException(nameof(IFileSink));
        }

        var now = DateTime.Now;

        AlignCurrentFileTo(now);

        while(_sink?.Write(context) == false)
        {
            AlignCurrentFileTo(now, true);
        }
    }

    /// <summary>
    /// Disposes the.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        lock(_lock)
        {
            if(_sink == null)
            {
                return;
            }

            CloseFile();

            _disposed = true;
        }
    }

    /// <summary>
    /// Aligns the current file to.
    /// </summary>
    /// <param name="datetime">The datetime.</param>
    /// <param name="nextSequence">If true, next sequence.</param>
    private void AlignCurrentFileTo(DateTime datetime, bool nextSequence = false)
    {
        if(_nextPeroid.HasValue == false)
        {
            OpenFile(datetime);
        }
        else if(nextSequence || datetime >= _nextPeroid.Value)
        {
            int? minSequence = null;

            if(nextSequence)
            {
                if(_currentSequence.HasValue == false)
                {
                    minSequence = 1;
                }
                else
                {
                    minSequence = _currentSequence.Value + 1;
                }

                CloseFile();

                OpenFile(datetime, minSequence);
            }
        }
    }

    /// <summary>
    /// Closes the file.
    /// </summary>
    private void CloseFile()
    {
        if(_sink != null)
        {
            (_sink as IDisposable)?.Dispose();
            _sink = null;
        }

        _nextPeroid = null;
    }

    /// <summary>
    /// Opens the file.
    /// </summary>
    /// <param name="now">The now.</param>
    /// <param name="minSequence">The min sequence.</param>
    private void OpenFile(DateTime now, int? minSequence = null)
    {
        var currentPeriod = _roller.GetCurrentPeriod(now);

        _nextPeroid = _roller.GetNextPeriod(now) ?? now.AddMinutes(30);

        var fileNames = Directory.GetFiles(_roller.PathDirectory, _roller.DirectorySearchPattern)
                .Select(x => Path.GetFileName(x));

        var latestFile = _roller
            .SelectMatches(fileNames)
            .Where(x => x.DateTime == currentPeriod)
            .OrderByDescending(x => x.SequenceNumber)
            .FirstOrDefault();

        var sequence = latestFile.SequenceNumber;
        if(minSequence.HasValue
            && (sequence.HasValue == false || sequence.Value < minSequence.Value))
        {
            sequence = minSequence;
        }

        var path = _roller.GetLogFilePath(now, sequence);

        var tryCount = 3;
        for(var i = 0; i < tryCount; i++)
        {
            try
            {
                // TODO: 滚动需要每次都重新创建吗？
                _sink = _shared
                    ? new SharedFileSink(path, _renderer, _fileSizeMaxBytes, _encoding)
                : new FileSink(path, _renderer, _fileSizeMaxBytes, _encoding, _buffered);

                _currentSequence = sequence;
            }
            catch(IOException)
            {
                continue;
            }

            // 应用保留策略
            ApplyRetentionPolicy(path, now);
            break;
        }
    }

    /// <summary>
    /// 应用保留策略
    /// </summary>
    /// <param name="path"></param>
    /// <param name="datetime"></param>
    private void ApplyRetentionPolicy(string path, DateTime datetime)
    {
        if(_retainedFileCountMax is null
            && _retainedFileTimeMax is null)
        {
            return;
        }

        var fileName = Path.GetFileName(path);
        var potentialMatches = Directory.GetFiles(_roller.PathDirectory, _roller.DirectorySearchPattern)
            .Select(x => Path.GetFileName(x))
            .Union(new[] { fileName });

        var newestFile = _roller
            .SelectMatches(potentialMatches)
            .OrderByDescending(x => x.DateTime)
            .ThenByDescending(x => x.SequenceNumber);

        var removeFiles = newestFile
            .Where(x => StringComparer.OrdinalIgnoreCase.Compare(fileName, x.Filename) != 0)
            .SkipWhile((x, i) => ShouldRetained(x, i + 1, datetime)).ToList();

        foreach(var removeFile in removeFiles)
        {
            var file = Path.Combine(_roller.PathDirectory, removeFile.Filename);
            try
            {
                File.Delete(file);
            }
            catch { }
        }
    }

    /// <summary>
    /// 文件是否应该保留
    /// </summary>
    /// <param name="file"></param>
    /// <param name="count"></param>
    /// <param name="datetime"></param>
    /// <returns></returns>
    private bool ShouldRetained(RollFile file, int count, DateTime datetime)
    {
        if(_retainedFileCountMax.HasValue
            && count > _retainedFileCountMax.Value)
        {
            return false;
        }

        if(_retainedFileTimeMax.HasValue
            && file.DateTime.HasValue
            && file.DateTime.Value < datetime.Subtract(_retainedFileTimeMax.Value))
        {
            return false;
        }

        return true;
    }
}
