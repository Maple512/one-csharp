namespace OneI.Logable.Rolling;

using System;
using System.IO;
using System.Linq;
using System.Text;
using OneI.Logable.Formatting;

public class RollingFileSink : ILoggerSink, IFileFlusher, IDisposable
{
    private readonly PathRoller _roller;
    private readonly ITextFormatter _textFormatter;
    private readonly long? _fileMaxSizeBytes;
    private readonly int? _retainedFileMaxCount;
    private readonly TimeSpan? _retainedFileMaxTime;
    private readonly Encoding? _encoding;
    private readonly bool _buffered;
    private readonly bool _shared;
    private readonly bool _rollOnFileMaxSize;
    private readonly FileLifecycleHooks? _hooks;
    private static readonly object _lock = new();
    private readonly bool _isDisposed;
    private DateTime? _nextCheckPoint;
    private IFileSink? _currentFileSink;
    private int? _currentSequence;

    public RollingFileSink(
        string path,
        ITextFormatter textFormatter,
        long? fileMaxSizeBytes,
        int? retainedFileMaxCount,
        Encoding? encoding,
        bool buffered,
        bool shared,
        RollingPolicy policy,
        bool rollOnFileSizeLimit,
        FileLifecycleHooks? hooks,
        TimeSpan? retainedFileMaxTime)
    {
        CheckTools.NotNull(path);

        _roller = new PathRoller(path, policy);
        _textFormatter = textFormatter;
        _fileMaxSizeBytes = fileMaxSizeBytes;
        _retainedFileMaxCount = retainedFileMaxCount;
        _retainedFileMaxTime = retainedFileMaxTime;
        _encoding = encoding;
        _buffered = buffered;
        _shared = shared;
        _rollOnFileMaxSize = rollOnFileSizeLimit;
        _hooks = hooks;
    }

    public void Emit(LoggerContext context)
    {
        lock(_lock)
        {
            if(_isDisposed)
            {
                throw new ObjectDisposedException("The log file has been disposed");
            }

            var now = DateTime.UtcNow;

            AlignCurrentFileTo(now);

            while(_currentFileSink?.EmitOrOverflow(context) == false
                && _rollOnFileMaxSize)
            {
                AlignCurrentFileTo(now, true);
            }
        }
    }

    public void Flush()
    {
        lock(_lock)
        {
            _currentFileSink?.Flush();
        }
    }

    public void Dispose()
    {
        lock(_lock)
        {
            CloseFile();
        }

        GC.SuppressFinalize(this);
    }

    private void AlignCurrentFileTo(DateTime now, bool nextSequence = false)
    {
        if(_nextCheckPoint.HasValue == false)
        {
            OpenFile(now);
        }
        else if(nextSequence
           || now >= _nextCheckPoint.Value)
        {
            int? minSequence = null;
            if(nextSequence)
            {
                if(_currentSequence == null)
                {
                    minSequence = 1;
                }
                else
                {
                    minSequence = _currentSequence.Value + 1;
                }
            }

            CloseFile();

            OpenFile(now, minSequence);
        }
    }

    private void OpenFile(DateTime now, int? minSequence = null)
    {
        var currentCheckPoint = _roller.GetCurrentCheckpoint(now);

        _nextCheckPoint = _roller.GetNextCheckpoint(now) ?? now.AddMinutes(30);

        var existingFiles = Enumerable.Empty<string>();
        try
        {
            if(Directory.Exists(_roller.FileDirectory))
            {
                existingFiles = Directory.GetFiles(_roller.FileDirectory, _roller.DirectorySearchPattern)
                    .Select(x => Path.GetFileName(x));
            }
        }
        catch(DirectoryNotFoundException) { }

        var file = _roller
            .SelectMatches(existingFiles)
            .Where(x => x.DateTime == currentCheckPoint)
            .OrderByDescending(x => x.SequenceNumber)
            .FirstOrDefault();

        var sequence = file.SequenceNumber;
        if(minSequence.HasValue)
        {
            if(sequence == null
                || sequence.Value < minSequence.Value)
            {
                sequence = minSequence;
            }
        }

        const int maxAttempts = 3;
        for(var attempt = 0; attempt < maxAttempts; attempt++)
        {
            _roller.GetFilePath(now, sequence, out var path);

            try
            {
                _currentFileSink = _shared
                    ? new SharedFileSink(path, _textFormatter, _fileMaxSizeBytes, _encoding)
                    : new FileSink(path, _textFormatter, _fileMaxSizeBytes, _buffered, _encoding, _hooks);

                _currentSequence = sequence;
            }
            catch(IOException)
            {
                InternalLog.WriteLine("File target {0} was locked, attempting to open next in sequence (attempt {1})", path, attempt + 1);

                sequence = (sequence ?? 0) + 1;

                continue;
            }

            ApplyRetentionPolicy(path, now);

            return;
        }
    }

    /// <summary>
    /// 文件保留策略
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="now"></param>
    private void ApplyRetentionPolicy(string filePath, DateTime now)
    {
        if(_retainedFileMaxCount == null
            && _retainedFileMaxTime == null)
        {
            return;
        }

        var fileName = Path.GetFileName(filePath);

        // 我们认为当前文件是存在的，即使尚未写入任何内容，因为文件仅在响应正在处理的事件时打开。
        var potentialMatches = Directory.GetFiles(_roller.FileDirectory, _roller.DirectorySearchPattern)
            .Select(x => Path.GetFileName(x))
            .Union(new[] { fileName });

        var matchedFiles = _roller.SelectMatches(potentialMatches)
            .OrderByDescending(x => x.DateTime)
            .ThenByDescending(x => x.SequenceNumber);

        var obsoleteFiles = matchedFiles
            .Where(x => fileName.Equals(x.Name, StringComparison.OrdinalIgnoreCase) == false)
            .SkipWhile((f, i) => ShouldRetainFile(f, i, now))
            .Select(x => x.Name)
            .ToList();

        foreach(var file in obsoleteFiles)
        {
            var fullPath = Path.Combine(_roller.FileDirectory, file);
            try
            {
                _hooks?.OnFileDeleting(fullPath);

                File.Delete(fullPath);
            }
            catch(Exception ex)
            {
                InternalLog.WriteLine($"Error {ex} while porocessing obsolete log file {fullPath}");
            }
        }
    }

    /// <summary>
    /// 是否应保留文件
    /// </summary>
    /// <param name="file"></param>
    /// <param name="index"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    private bool ShouldRetainFile(RollingFile file, int index, DateTime now)
    {
        if(_retainedFileMaxCount.HasValue
            && index >= _retainedFileMaxCount - 1)
        {
            return false;
        }

        if(_retainedFileMaxTime.HasValue
            && file.DateTime.HasValue
            && file.DateTime.Value < now.Subtract(_retainedFileMaxTime.Value))
        {
            return false;
        }

        return true;
    }

    private void CloseFile()
    {
        (_currentFileSink as IDisposable)?.Dispose();

        _currentFileSink = null;

        _nextCheckPoint = null;
    }
}
