namespace OneI.Logable;

using System;
using System.IO;
using System.Text;
using System.Threading;
using OneI.Logable.Formatting;

public class SharedFileSink : IFileSink, IDisposable
{
    private readonly TextWriter _output;
    private readonly FileStream _underlyingStream;
    private readonly ITextFormatter _textFormatter;
    private readonly long? _fileMaxSizeBytes;
    private static readonly object _lock = new();
    private const string MutexNameSuffix = ".serilog";
    private const int MutexWaitTimeout = 10_000;

    // 互斥锁，确保共享对象不会同时被多个线程访问
    private readonly Mutex _mutex;

    public SharedFileSink(string path,
        ITextFormatter textFormatter,
        long? fileMaxSizeBytes,
        Encoding? encoding = null)
    {
        if(path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        _textFormatter = textFormatter;
        _fileMaxSizeBytes = fileMaxSizeBytes;

        IOTools.EnsureDirectoryExisted(path);

        var mutexName = $"{Path.GetFullPath(path).Replace(Path.DirectorySeparatorChar, ':')}{MutexNameSuffix}";

        _mutex = new Mutex(false, mutexName);

        _underlyingStream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

        _output = new StreamWriter(_underlyingStream, encoding ?? Encoding.UTF8);
    }

    bool IFileSink.EmitOrOverflow(LoggerContext context)
    {
        lock(_lock)
        {
            if(TryAcquireMutex() == false)
            {
                return true;
            }

            try
            {
                _underlyingStream.Seek(0, SeekOrigin.End);

                if(_fileMaxSizeBytes.HasValue)
                {
                    if(_underlyingStream.Length >= _fileMaxSizeBytes.Value)
                    {
                        return false;
                    }
                }

                _textFormatter.Format(context, _output);

                _output.Flush();

                _underlyingStream.Flush();

                return true;
            }
            finally
            {
                ReleaseMutex();
            }
        }
    }

    public void Emit(LoggerContext context)
    {
        ((IFileSink)this).EmitOrOverflow(context);
    }

    public void Flush()
    {
        lock(_lock)
        {
            if(TryAcquireMutex() == false)
            {
                return;
            }

            try
            {
                _underlyingStream.Flush(true);
            }
            finally
            {
                ReleaseMutex();
            }
        }
    }

    public void Dispose()
    {
        lock(_lock)
        {
            _output.Dispose();

            _mutex.Dispose();
        }
    }

    private bool TryAcquireMutex()
    {
        try
        {
            if(_mutex.WaitOne(MutexWaitTimeout) == false)
            {
                InternalLog.WriteLine("Shared file mutex could not be acquired within {0} ms", MutexWaitTimeout);

                return false;
            }
        }
        catch(AbandonedMutexException)
        {
            InternalLog.WriteLine("Inherited shared file mutex after abandonment by another process");
        }

        return true;
    }

    private void ReleaseMutex() => _mutex.ReleaseMutex();
}
