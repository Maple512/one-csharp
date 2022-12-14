namespace OneI.Logable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SharedFileSink : IFileSink, IDisposable
{
    readonly TextWriter _writer;
    readonly FileStream _stream;
    readonly ITextRenderer _textRenderer;
    readonly long? _fileSizeMaxBytes;

    static readonly object _lock = new();

    const string MutexNameSuffix = ".logable";

    // 互斥锁等待时间（ms）
    const int MutexWaitTimeout = 10_000;
    readonly Mutex _mutex;

    public SharedFileSink(
        string path,
        ITextRenderer textRenderer,
        long? fileSizeMaxBytes,
        Encoding? encoding)
    {
        _textRenderer = textRenderer;
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

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        lock(_lock)
        {
            _stream.Dispose();
            _mutex.Dispose();
        }
    }

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

    public void Invoke(in LoggerContext context)
    {
        Write(context);
    }

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

                _textRenderer.Render(context, _writer);

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
    bool TryAcquireMutex()
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
