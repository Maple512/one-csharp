namespace OneI.Logable;

using System;
using Microsoft.Win32.SafeHandles;
using OneI.Logable.Internal;
using OneI.Logable.Rendering;
using OneI.Logable.Templatizations;
using OneI.Logable.Templatizations.Tokenizations;

internal class FileSink : ILoggerSink, IDisposable
{
    private FileWriter? _writer;
    private readonly FileQueue _queue;
    private readonly ILoggerRenderer _renderer;
    private DateTime? _nextPeroid;
    private readonly LogFileOptions _options;

    private static readonly object _lock = new();

    public FileSink(LogFileOptions options)
    {
        _options = options;
        _queue = new FileQueue(options.CountLimit);
        _renderer = options.Renderer;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        lock(_lock)
        {
            _queue.Dispose();

            _nextPeroid = null;
        }
    }

    public void Invoke(in LoggerContext context)
    {
        lock(_lock)
        {
            AlignFile(context, DateTime.Now);

            _renderer.Render(context, _writer!);
        }
    }

    private void AlignFile(in LoggerContext context, DateTime datetime)
    {
        _nextPeroid ??= _options.Frequency.GetNextPeriod(datetime);

        // 初始化
        if(_writer is null)
        {
            var file = GetCurrentFile(context, datetime, true);

            _writer = new FileWriter(file, _options.Encoding);
        }

        // 长度限制 or 下个周期
        if(_options.SizeLimit.HasValue && _options.SizeLimit.Value <= _writer.Position
            || _nextPeroid.HasValue && datetime > _nextPeroid.Value)
        {
            _writer.Dispose();

            var file = GetCurrentFile(context, datetime);

            _writer.ResetNewFile(file);

            _nextPeroid = _options.Frequency.GetNextPeriod(datetime);
        }

        // 时间过期
        if(_options.ExpiredTime is not null)
        {
            _queue.DeleteExpiredFiles(DateTime.Now.Subtract(_options.ExpiredTime.Value));
        }
    }

    private SafeFileHandle GetCurrentFile(in LoggerContext context, DateTime datetime, bool init = false)
    {
        var path = ParseFilePath(context);

        var directory = Path.GetDirectoryName(path)!;
        var name = Path.GetFileNameWithoutExtension(path);
        var extension = Path.GetExtension(path);

        var index = 0;
        string fullPath;

        do
        {
            var count = index > 0 ? 1 : 0;
            var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(count, 3 + count);
            defaultInterpolatedStringHandler.AppendFormatted(name);
            defaultInterpolatedStringHandler.AppendFormatted(datetime.ToString(RollFrequencyExtensions.GetFormat(_options.Frequency)));
            if(index > 0)
            {
                defaultInterpolatedStringHandler.AppendLiteral("_");
                defaultInterpolatedStringHandler.AppendFormatted<int>(index, "000");
            }

            defaultInterpolatedStringHandler.AppendFormatted(extension);
            fullPath = Path.Combine(directory, defaultInterpolatedStringHandler.ToStringAndClear());
            index++;
        } while(File.Exists(fullPath));

        var file = _queue.GetNewFile(fullPath, _options.SizeLimit);

        return file;
    }

    private string ParseFilePath(in LoggerContext context)
    {
        if(_options.Tokens.Count == 1 && _options.Tokens[0] is TextToken tt)
        {
            return tt.Text;
        }

        var container = new StringWriter(new StringBuilder(256));

        var template = new TemplateContext(_options.Tokens, context.Properties);

        template.Render(container);

        return container.ToString();
    }
}
