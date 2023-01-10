namespace OneI.Logable;

using System;
using System.Text;
/// <summary>
/// The shared file sink.
/// </summary>

internal class SharedFileSink : FileSinkBase, IFileSink, IDisposable
{
    private readonly TextWriter _writer;
    private readonly FileStream _stream;
    private readonly long? _fileSizeMaxBytes;
    private static readonly object _lock = new();
    /// <summary>
    /// The mutex name suffix.
    /// </summary>
    private const string MutexNameSuffix = ".logable";

    // 互斥锁等待时间（ms）
    /// <summary>
    /// The mutex wait timeout.
    /// </summary>
    private const int MutexWaitTimeout = 10_000;
    private readonly Mutex _mutex;

    /// <summary>
    /// Initializes a new instance of the <see cref="SharedFileSink"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="rendererProvider">The renderer provider.</param>
    /// <param name="fileSizeMaxBytes">The file size max bytes.</param>
    /// <param name="encoding">The encoding.</param>
    public SharedFileSink(
        string path,
        ITextRendererProvider rendererProvider,
        long? fileSizeMaxBytes,
        Encoding? encoding) : base(rendererProvider)
    {
        _fileSizeMaxBytes = fileSizeMaxBytes;

        var directory = Path.GetDirectoryName(path);
        if(directory.IsNullOrWhiteSpace())
        {
            directory = Directory.GetCurrentDirectory();
        }

        if(Directory.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }

        _stream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

        _writer = new StreamWriter(_stream, encoding ?? Encoding.UTF8);

        var fullPath = Path.GetFullPath(path).Replace(Path.DirectorySeparatorChar, ':');
        _mutex = new Mutex(false, $"{fullPath}.{MutexNameSuffix}");
    }

    /// <summary>
    /// Disposes the.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        lock(_lock)
        {
            _stream.Dispose();
            _mutex.Dispose();
        }
    }

    /// <summary>
    /// Flushes the to disk.
    /// </summary>
    public void FlushToDisk()
    {
        lock(_lock)
        {
            if(TryAcquireMutex() == false)
            {
                return;
            }

            try
            {
                _stream.Flush(true);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
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
            if(TryAcquireMutex() == false)
            {
                return true;
            }

            try
            {
                // 重新定位文件末尾
                _stream.Seek(0, SeekOrigin.End);

                if(_fileSizeMaxBytes.HasValue
                    && _stream.Length >= _fileSizeMaxBytes.Value)
                {
                    return false;
                }

                Render(context, _writer);

                // 共享文件，不需要缓冲区
                _writer.Flush();
                _stream.Flush();

                return true;
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }
    }

    /// <summary>
    /// 尝试获取互斥信号
    /// </summary>
    /// <returns></returns>
    private bool TryAcquireMutex()
    {
        try
        {
            if(!_mutex.WaitOne(MutexWaitTimeout))
            {
                return false;
            }
        }
        catch(AbandonedMutexException)
        {

        }

        return true;
    }
}
