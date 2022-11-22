namespace OneI.Logable;

using System;
using System.IO;
using System.Text;
using OneI.Logable.Formatting;

public class FileSink : IFileSink, IDisposable
{
    private readonly TextWriter _output;
    private readonly FileStream _underlyingStream;
    private readonly ITextFormatter _textFormatter;
    private readonly long? _fileMaxBytes;
    private readonly bool _buffered;
    private readonly StreamCounter? _streamCounter;
    private static readonly object _lock = new();

    public FileSink(
        string path,
        ITextFormatter textFormatter,
        long? fileMaxBytes,
        bool buffered,
        Encoding? encoding,
        FileLifecycleHooks? hooks = null)
    {
        _textFormatter = textFormatter;
        _fileMaxBytes = fileMaxBytes;
        _buffered = buffered;

        IOTools.EnsureDirectoryExisted(path);

        Stream outputStream = _underlyingStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        // 将写入位置定位到文件末尾
        outputStream.Seek(0, SeekOrigin.End);

        if(_fileMaxBytes.HasValue)
        {
            outputStream = _streamCounter = new StreamCounter(_underlyingStream);
        }

        encoding ??= Encoding.UTF8;

        if(hooks != null)
        {
            outputStream = hooks.OnFileOpened(path, outputStream, encoding)
                ?? throw new InvalidOperationException();
        }

        _output = new StreamWriter(outputStream, encoding);
    }

    public void Emit(LoggerContext context)
    {
        ((IFileSink)this).EmitOrOverflow(context);
    }

    /// <summary>
    /// 清除此流的缓冲区（包括中间缓冲区），使得所有缓冲数据都写入到文件中
    /// </summary>
    public void Flush()
    {
        lock(_lock)
        {
            _output.Flush();

            _underlyingStream.Flush(true);
        }
    }

    public void Dispose()
    {
        lock(_lock)
        {
            _output.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    bool IFileSink.EmitOrOverflow(LoggerContext context)
    {
        if(context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        lock(_lock)
        {
            if(_fileMaxBytes.HasValue)
            {
                if(_streamCounter!.Count >= _fileMaxBytes.Value)
                {
                    return false;
                }
            }

            _textFormatter.Format(context, _output);

            if(_buffered == false)
            {
                _output.Flush();
            }

            return true;
        }
    }
}
