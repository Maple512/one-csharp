namespace OneI.Logable;

using System;
using Internal;
using Rendering;
using Templatizations;
using Templatizations.Tokenizations;

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
        _files = new((int)options.CountLimit);
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

        // 初始化
        _writer ??= InlitializeFileStream(context, datetime, true);

        // 长度限制
        if(_options.SizeLimit > 0 && _options.SizeLimit <= (ulong)_counter!.Position)
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
            DeleteFiles(_files.Take(_files.Count - (int)_options.CountLimit));
        }
    }

    private void OpenFile(LoggerContext context, DateTime datetime)
    {
        _writer!.Dispose();

        _writer = InlitializeFileStream(context, datetime, false);
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

    private StreamWriter InlitializeFileStream(LoggerContext context, DateTime datetime, bool init)
    {
        var path = ParseFilePath(context);

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
            Options = FileOptions.RandomAccess | FileOptions.Asynchronous,
            Share = FileShare.Read,
            PreallocationSize = (long)_options.SizeLimit,
            BufferSize = (int)_options.BufferSize,
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

    private string ParseFilePath(LoggerContext context)
    {
        if(_options.Tokens is [TextToken tt])
        {
            return tt.Text;
        }

        using var writer = new StringWriter(new StringBuilder(10));

        TemplateContext.Render(writer, _options.Tokens, context.MessageContext);

        return writer.ToString();
    }
}
