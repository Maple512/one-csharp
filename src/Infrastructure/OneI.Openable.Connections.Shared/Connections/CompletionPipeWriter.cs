namespace OneI.Openable.Connections;

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

internal sealed class CompletionPipeWriter : PipeWriter
{
    private PipeWriter _writer;

    public bool IsCompleted { get; private set; }
    public Exception? CompleteException { get; private set; }
    public bool IsCompletedSuccessfully => IsCompleted && CompleteException == null;

    public CompletionPipeWriter(PipeWriter inner)
    {
        _writer = inner;
    }

    public override void Advance(int bytes)
    {
        _writer.Advance(bytes);
    }

    public override void CancelPendingFlush()
    {
        _writer.CancelPendingFlush();
    }

    public override void Complete(Exception? exception = null)
    {
        IsCompleted = true;
        CompleteException = exception;
        _writer.Complete(exception);
    }

    public override ValueTask CompleteAsync(Exception? exception = null)
    {
        IsCompleted = true;
        CompleteException = exception;
        return _writer.CompleteAsync(exception);
    }

    public override ValueTask<FlushResult> WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default)
    {
        return _writer.WriteAsync(source, cancellationToken);
    }

    public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
    {
        return _writer.FlushAsync(cancellationToken);
    }

    public override Memory<byte> GetMemory(int sizeHint = 0)
    {
        return _writer.GetMemory(sizeHint);
    }

    public override Span<byte> GetSpan(int sizeHint = 0)
    {
        return _writer.GetSpan(sizeHint);
    }

    public void Reset()
    {
        IsCompleted = false;
        CompleteException = null;
    }
}
