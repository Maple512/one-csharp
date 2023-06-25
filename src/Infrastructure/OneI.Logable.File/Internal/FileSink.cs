namespace OneI.Logable.Internal;

using System;
using DotNext.IO;

internal class FileSink : ILoggerSink, IDisposable
{
    private readonly LogFileOptions _options;
    private readonly TextWriter _writer;

    public FileSink(LogFileOptions options)
    {
        _options = options;

        var file = GetNewFile(options);

        _writer = new FileBufferingWriter(new FileBufferingWriter.Options
        {
            AsyncIO = false,
            FileBufferSize = options.BufferSize,
            FileName = file,
        }).AsTextWriter(options.Encoding);
    }

    public void Dispose()
    {
        Debugger.Break();

        _writer.Dispose();
    }

    public void Invoke(in LoggerContext context)
    {
        lock(_writer)
        {
            context.WriteTo(_writer, _options.FormatProvider);
        }
    }

    public static string GetNewFile(LogFileOptions options, string? suffix = null)
    {
        var fileInfo = Path.Combine(options.Directory, $"{options.FileName}{suffix}{options.FileExtensions}");
        var sequenceNo = 0;
        while(File.Exists(fileInfo))
        {
            try
            {
                fileInfo = Path.Combine(options.Directory, $"{options.FileName}{suffix}_{++sequenceNo:000}{options.FileExtensions}");
            }
            catch { }
        }

        return fileInfo;
    }
}
