namespace OneI.Logable;
/// <summary>
/// The file sink.
/// </summary>

internal class FileSink : FileSinkBase, IFileSink, IDisposable
{
    private readonly TextWriter _writer;
    private readonly FileStream _originalStream;
    private readonly long? _fileSizeMaxBytes;
    private readonly bool _buffered;
    private readonly StreamCounter? _counter;

    private static readonly object _lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSink"/> class.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="rendererProvider"></param>
    /// <param name="fileSizeMaxBytes">文件最大长度</param>
    /// <param name="encoding"></param>
    /// <param name="buffered">是否开启缓冲</param>
    public FileSink(
        string path,
        ITextRendererProvider rendererProvider,
        long? fileSizeMaxBytes,
        Encoding? encoding,
        bool buffered) : base(rendererProvider)
    {
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

    /// <summary>
    /// Flushes the to disk.
    /// </summary>
    public void FlushToDisk()
    {
        lock(_lock)
        {
            _writer.Flush();
            _originalStream.Flush(true);
        }
    }

    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void Invoke(in LoggerContext context)
    {
        Write(context);
    }

    /// <summary>
    /// Writes the.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A bool.</returns>
    public bool Write(in LoggerContext context)
    {
        lock(_lock)
        {
            if(_fileSizeMaxBytes.HasValue
                && _fileSizeMaxBytes.Value <= _counter!.TotalLength)
            {
                return false;
            }

            GetTextRenderer(context).Render(context, _writer);

            if(!_buffered)
            {
                _writer.Flush();
            }

            return true;
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
            _writer.Dispose();
        }
    }
}
