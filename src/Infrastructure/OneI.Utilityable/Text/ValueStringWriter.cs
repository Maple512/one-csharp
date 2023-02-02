namespace OneI.Text;

using System.Globalization;
using Cysharp.Text;

/// <summary>
/// source: <see href="https://github.com/Cysharp/ZString/blob/master/src/ZString/ZStringWriter.cs"/>
/// </summary>
public sealed class ValueStringWriter : TextWriter
{
    private Utf16ValueStringBuilder container;
    private bool isOpen;

    /// <summary>
    /// Creates a new instance using <see cref="CultureInfo.CurrentCulture"/> as format provider.
    /// </summary>
    public ValueStringWriter() : this(CultureInfo.CurrentCulture)
    {
    }

    /// <summary>
    /// Creates a new instance with given format provider.
    /// </summary>
    public ValueStringWriter(IFormatProvider formatProvider) : base(formatProvider)
    {
        container = ZString.CreateStringBuilder();
        isOpen = true;
    }

    /// <summary>
    /// Disposes this instance, operations are no longer allowed.
    /// </summary>
    public override void Close()
    {
        Dispose(true);
    }

    protected override void Dispose(bool disposing)
    {
        container.Dispose();

        isOpen = false;
    }

    public override Encoding Encoding => Encoding.UTF8;

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => container.Length;
    }

    public override void Write(char value)
    {
        AssertNotDisposed();

        container.Append(value);
    }

    public override void Write(char[] buffer, int index, int count)
    {
        if(buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if(index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if(count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if(buffer.Length - index < count)
        {
            throw new ArgumentException();
        }

        AssertNotDisposed();

        container.Append(buffer, index, count);
    }

    public override void Write(string? value)
    {
        AssertNotDisposed();

        if(value is { Length: > 0 })
        {
            container.Append(value);
        }
    }

    public override Task WriteAsync(char value)
    {
        Write(value);
        return Task.CompletedTask;
    }

    public override Task WriteAsync(string? value)
    {
        Write(value);
        return Task.CompletedTask;
    }

    public override Task WriteAsync(char[] buffer, int index, int count)
    {
        Write(buffer, index, count);
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(char value)
    {
        WriteLine(value);
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(string? value)
    {
        WriteLine(value);
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(char[] buffer, int index, int count)
    {
        WriteLine(buffer, index, count);
        return Task.CompletedTask;
    }

    public override void Write(bool value)
    {
        AssertNotDisposed();
        container.Append(value);
    }

    public override void Write(decimal value)
    {
        AssertNotDisposed();
        container.Append(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        container.Clear();
    }

    /// <summary>
    /// No-op.
    /// </summary>
    public override Task FlushAsync()
    {
        return Task.CompletedTask;
    }

    public ReadOnlySpan<char> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => container.AsSpan();
    }

    /// <summary>
    /// Materializes the current state from underlying string builder.
    /// </summary>
    public override string ToString()
    {
        return container.ToString();
    }

    public override void Write(ReadOnlySpan<char> buffer)
    {
        AssertNotDisposed();

        container.Append(buffer);
    }

    public override void WriteLine(ReadOnlySpan<char> buffer)
    {
        AssertNotDisposed();

        container.Append(buffer);
        WriteLine();
    }

    public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        Write(buffer.Span);
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        WriteLine(buffer.Span);
        return Task.CompletedTask;
    }

    private void AssertNotDisposed()
    {
        if(!isOpen)
        {
            throw new ObjectDisposedException(nameof(container));
        }
    }
}
