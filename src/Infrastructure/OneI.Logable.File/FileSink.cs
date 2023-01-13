namespace OneI.Logable;

using OneI.Logable.Rendering;

internal class FileSink : ILoggerSink, IFileSink, IDisposable
{
    private readonly TextWriter _writer;
    private readonly FileStream _originalStream;
    private readonly long? _fileSizeMaxBytes;
    private readonly bool _buffered;
    private readonly StreamCounter? _counter;
    private readonly ILoggerRenderer _renderer;

    private static readonly object _lock = new();

    public FileSink(
        string path,
        ILoggerRenderer renderer,
        long? fileSizeMaxBytes,
        Encoding? encoding,
        bool buffered)
    {
        _renderer = renderer;
        _fileSizeMaxBytes = fileSizeMaxBytes;
        _buffered = buffered;

        var directory = Path.GetDirectoryName(path);
        if(directory.IsNullOrWhiteSpace())
        {
            directory = Directory.GetCurrentDirectory();
        }
        else if(Directory.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }

        Stream stream = _originalStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        stream.Seek(0, SeekOrigin.End);

        if(_fileSizeMaxBytes.HasValue)
        {
            stream = _counter = new StreamCounter(_originalStream);
        }

        encoding ??= Encoding.UTF8;

        _writer = new StreamWriter(stream, encoding);
    }

    public void FlushToDisk()
    {
        lock(_lock)
        {
            _writer.Flush();
            _originalStream.Flush(true);
        }
    }

    public void Invoke(in LoggerContext context)
    {
        Write(context);
    }

    public bool Write(in LoggerContext context)
    {
        lock(_lock)
        {
            if(_fileSizeMaxBytes.HasValue
                && _fileSizeMaxBytes.Value <= _counter!.TotalLength)
            {
                return false;
            }

            _renderer.Render(context, _writer);

            if(!_buffered)
            {
                _writer.Flush();
            }

            return true;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        lock(_lock)
        {
            _writer.Dispose();
        }
    }
}
