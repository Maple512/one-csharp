namespace OneI.Logable;

using System;
using Internal;
using Templates;

internal class FileSink : ILoggerSink, IDisposable
{
    private TextWriter? _writer;
    private StreamCounter? _counter;
    private readonly ILoggerRenderer _renderer;
    private DateTime? _nextPeroid;
    private readonly LogFileOptions _options;
    private readonly List<FileItem> _files;

    private static readonly object _lock = new();

    public FileSink(LogFileOptions options)
    {
        _options = options;
        _files = new(options.CountLimit);
        _renderer = options.Renderer;
    }

    public void Dispose()
    {
        lock(_lock)
        {
            _writer?.Dispose();

            _nextPeroid = null;
        }
    }

    public void Invoke(LoggerContext context)
    {
        lock(_lock)
        {
            AlignFile(context, DateTime.Now);

            _renderer.Render(context, _writer!);
        }
    }

    private void AlignFile(LoggerContext context, DateTime datetime)
    {
        _nextPeroid ??= _options.Frequency.GetNextPeriod(datetime);

        _writer ??= InlitializeTextWriter(context, datetime, true);

        // 长度限制
        if(_options.SizeLimit > 0 && _options.SizeLimit <= _counter!.Position)
        {
            OpenFile(context, datetime);
        }

        // 下个周期
        if(datetime > _nextPeroid)
        {
            OpenFile(context, datetime);

            _nextPeroid = _options.Frequency.GetNextPeriod(datetime);
        }

        // 时间限制
        if(_options.ExpiredTime != TimeSpan.Zero)
        {
            var expired = datetime.Subtract(_options.ExpiredTime);

            DeleteFiles(_files.Where(x => x.CreatedAt < expired));
        }

        // 数量限制
        if(_options.CountLimit > 1
            && _files.Count > _options.CountLimit)
        {
            DeleteFiles(_files.Take(_files.Count - _options.CountLimit));
        }
    }

    
    private void OpenFile(LoggerContext context, DateTime datetime)
    {
        _writer!.Dispose();

        _writer = InlitializeTextWriter(context, datetime, false);
    }

    private static void DeleteFiles(IEnumerable<FileItem> files)
    {
        foreach(var item in files)
        {
            try
            {
                File.Delete(item.FullPath);
            }
            catch { }
        }
    }

    private StreamWriter InlitializeTextWriter(LoggerContext context, DateTime datetime, bool init)
    {
        var path = _options.Path;

        var directory = Path.GetDirectoryName(path)!;
        var name = Path.GetFileNameWithoutExtension(path);
        var extension = Path.GetExtension(path);

        var fullPath = Path.Combine(directory, $"{name}{datetime.ToString(_options.Frequency.GetFormat())}{extension}");

        if(init)
        {
            IOTools.EnsureExistedDirectory(fullPath);
        }

        var options = new FileStreamOptions
        {
            Access = FileAccess.Write,
            Mode = FileMode.OpenOrCreate,
            //Options = FileOptions.RandomAccess | FileOptions.Asynchronous,
            Share = FileShare.Read,
            PreallocationSize = _options.SizeLimit,
            BufferSize = _options.BufferSize,
        };

        Stream stream = new FileStream(fullPath, options);
        if(init)
        {
            stream.Seek(0, SeekOrigin.End);
        }

        if(_options.SizeLimit > 0)
        {
            stream = _counter = new StreamCounter(stream);
        }

        _files.Add(new FileItem(fullPath));

        return new StreamWriter(stream);
    }
}
