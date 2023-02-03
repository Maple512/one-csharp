namespace OneI.Logable.Internal;

using System;
using System.IO;
using DotNext.IO;
using OneI.Logable.Templates;

internal class RollFileSink : ILoggerSink, IDisposable
{
    private readonly Queue<FileItem> _files;
    private readonly LogRollFileOptions _options;
    private DateTime _nextPeroid;
    private TextWriter _writer;
    private FileBufferingWriter _growableBuffer;
    private static readonly object _lock = new();

#pragma warning disable CS8618 
    public RollFileSink(LogRollFileOptions options)
    {
        _options = options;
        _files = new(options.CountLimit);
        _nextPeroid = _options.Frequency.GetNextPeriod(DateTime.Now);
    }
#pragma warning restore CS8618 

    public void Dispose()
    {
        lock(_lock)
        {
            if(_writer is not null)
            {
                _writer.Flush();
                _writer.Dispose();
            }

            _nextPeroid = default;
        }
    }

    public void Invoke(in LoggerContext context)
    {
        lock(_lock)
        {
            TryGetNewFile(DateTime.Now);

            TemplateRenderHelper.Render(_writer, context, _options.FormatProvider);
        }
    }

    private void TryGetNewFile(DateTime datetime)
    {
        if(_writer == null)
        {
            TrySetNextTextWriter(datetime, true);

            return;
        }

        // 周期
        if(datetime > _nextPeroid)
        {
            _nextPeroid = _options.Frequency.GetNextPeriod(datetime);
            TrySetNextTextWriter(datetime, true);
        }

        // 长度
        if(_options.SizeLimit > 0
             && (_options.SizeLimit * 1024 * 1024) <= _growableBuffer.Length)
        {
            TrySetNextTextWriter(datetime);
        }

        // 时间
        if(_options.ExpiredTime != TimeSpan.Zero)
        {
            var expired = datetime.Subtract(_options.ExpiredTime);

            for(var i = 0; i < _files.Count; i++)
            {
                var file = _files.Peek();
                if(file.CreatedAt < expired)
                {
                    DeleteFile(_files.Dequeue());
                    continue;
                }

                break;
            }
        }

        // 数量
        if(_options.CountLimit > 1
           && _files.Count > _options.CountLimit)
        {
            Debugger.Break();
            var count = _files.Count - _options.CountLimit;
            for(var i = 0; i < count; i++)
            {
                DeleteFile(_files.Dequeue());
            }
        }
    }

    private static void DeleteFile(FileItem file)
    {
        try
        {
            File.Delete(file.FullPath);
        }
        catch { }
    }

    private void TrySetNextTextWriter(DateTime datetime, bool init = false)
    {
        _writer?.Dispose();

        var fileSuffix = datetime.ToString(_options.Frequency.GetFormat());
        var file = GetNewFile(_options, fileSuffix);

        _files.Enqueue(new FileItem(file));

        _growableBuffer = new FileBufferingWriter(new FileBufferingWriter.Options
        {
            AsyncIO = false,
            FileBufferSize = _options.BufferSize,
            FileName = file,
            MemoryThreshold = _options.BufferSize,
        });

        _writer = _growableBuffer.AsTextWriter(_options.Encoding);
    }

    public static string GetNewFile(LogFileOptions options, string suffix)
    {
        var fileInfo = Path.Combine(options.Directory, $"{options.FileName}{suffix}{options.FileExtensions}");

        var sequenceNo = 0;
        while(File.Exists(fileInfo))
        {
            fileInfo = Path.Combine(options.Directory, $"{options.FileName}{suffix}_{++sequenceNo:000}{options.FileExtensions}");
        }

        return fileInfo;
    }
}
