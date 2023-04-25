namespace OneI.Openable.Connections;

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

internal sealed class CompletionPipeReader : PipeReader
{
    private PipeReader _reader;

    public CompletionPipeReader(PipeReader reader)
    {
        _reader = reader;
    }

    public bool IsCompleted { get; private set; }

    public Exception? CompleteException { get; private set; }

    public bool IsCompletedSuccessfully => IsCompleted && CompleteException is null;

    public override void AdvanceTo(SequencePosition consumed)
    {
        _reader.AdvanceTo(consumed);
    }

    public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
    {
        _reader.AdvanceTo(consumed, examined);
    }

    public override void CancelPendingRead()
    {
        _reader.CancelPendingRead();
    }

    public override void Complete(Exception? exception = null)
    {
        IsCompleted = true;
        CompleteException = exception;
        _reader.Complete(exception);
    }

    public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
    {
        return _reader.ReadAsync(cancellationToken);
    }

    public override bool TryRead(out ReadResult result)
    {
        return _reader.TryRead(out result);
    }

    public void Reset()
    {
        IsCompleted = false;
        CompleteException = null;
    }
}
