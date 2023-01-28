namespace OneI.Text;

using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

public class ValueStringWriter : TextWriter
{
    private static volatile UnicodeEncoding? s_encoding;

    private readonly ValueStringBuilder _container;
    private bool _isOpen;

    // Constructs a new StringWriter. A new StringBuilder is automatically
    // created and associated with the new StringWriter.
    public ValueStringWriter()
        : this(new ValueStringBuilder(), CultureInfo.CurrentCulture)
    {
    }

    public ValueStringWriter(IFormatProvider? formatProvider)
        : this(new ValueStringBuilder(), formatProvider)
    {
    }

    public ValueStringWriter(ValueStringBuilder container) : this(container, CultureInfo.CurrentCulture)
    {
    }

    public ValueStringWriter(ValueStringBuilder container, IFormatProvider? formatProvider) : base(formatProvider)
    {
        ArgumentNullException.ThrowIfNull(container);

        _container = container;
        _isOpen = true;
    }

    public override void Close()
    {
        Dispose(true);
    }

    protected override void Dispose(bool disposing)
    {
        // Do not destroy _sb, so that we can extract this after we are
        // done writing (similar to MemoryStream's GetBuffer & ToArray methods)
        _isOpen = false;
        base.Dispose(disposing);
    }

    public override Encoding Encoding => s_encoding ??= new UnicodeEncoding(false, false);

    public virtual ValueStringBuilder GetStringBuilder() => _container;

    // Writes a character to the underlying string buffer.
    //
    public override void Write(char value)
    {
        if(!_isOpen)
        {
            throw new ObjectDisposedException(null, "Cannot write to a closed TextWriter.");
        }

        _container.Append(value);
    }

    // Writes a range of a character array to the underlying string buffer.
    // This method will write count characters of data into this
    // StringWriter from the buffer character array starting at position
    // index.
    //
    public override void Write(char[] buffer, int index, int count)
    {
        if(buffer is null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.buffer);
        }

        if(index < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(index));
        }

        if(count < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(count));
        }

        if(buffer.Length - index < count)
        {
            throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
        }

        if(!_isOpen)
        {
            throw new ObjectDisposedException(null, "Cannot write to a closed TextWriter.");
        }

        _container.Append(buffer, index, count);
    }

    public override void Write(ReadOnlySpan<char> buffer)
    {
        if(GetType() != typeof(StringWriter))
        {
            // This overload was added after the Write(char[], ...) overload, and so in case
            // a derived type may have overridden it, we need to delegate to it, which the base does.
            base.Write(buffer);
            return;
        }

        if(!_isOpen)
        {
            throw new ObjectDisposedException(null, "Cannot write to a closed TextWriter.");
        }

        _container.Append(buffer);
    }

    // Writes a string to the underlying string buffer. If the given string is
    // null, nothing is written.
    //
    public override void Write(string? value)
    {
        if(!_isOpen)
        {
            throw new ObjectDisposedException(null, "Cannot write to a closed TextWriter.");
        }

        if(value != null)
        {
            _container.Append(value);
        }
    }

    public override void Write(StringBuilder? value)
    {
        if(GetType() != typeof(StringWriter))
        {
            // This overload was added after the Write(char[], ...) overload, and so in case
            // a derived type may have overridden it, we need to delegate to it, which the base does.
            base.Write(value);
            return;
        }

        if(!_isOpen)
        {
            throw new ObjectDisposedException(null, "Cannot write to a closed TextWriter.");
        }

        _container.Append(value);
    }

    public override void WriteLine(ReadOnlySpan<char> buffer)
    {
        if(GetType() != typeof(StringWriter))
        {
            // This overload was added after the WriteLine(char[], ...) overload, and so in case
            // a derived type may have overridden it, we need to delegate to it, which the base does.
            base.WriteLine(buffer);
            return;
        }

        if(!_isOpen)
        {
            throw new ObjectDisposedException(null, "Cannot write to a closed TextWriter.");
        }

        _container.Append(buffer);

        WriteLine();
    }

    public override void WriteLine(StringBuilder? value)
    {
        if(GetType() != typeof(StringWriter))
        {
            // This overload was added after the WriteLine(char[], ...) overload, and so in case
            // a derived type may have overridden it, we need to delegate to it, which the base does.
            base.WriteLine(value);
            return;
        }

        if(!_isOpen)
        {
            throw new ObjectDisposedException(null, "Cannot write to a closed TextWriter.");
        }

        _container.Append(value);
        WriteLine();
    }

    #region Task based Async APIs

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

    public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        Write(buffer.Span);
        return Task.CompletedTask;
    }

    public override Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default)
    {
        if(GetType() != typeof(StringWriter))
        {
            // This overload was added after the WriteAsync(char[], ...) overload, and so in case
            // a derived type may have overridden it, we need to delegate to it, which the base does.
            return base.WriteAsync(value, cancellationToken);
        }

        if(cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        if(!_isOpen)
        {
            throw new ObjectDisposedException(null, "Cannot write to a closed TextWriter.");
        }

        _container.Append(value);

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

    public override Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default)
    {
        if(GetType() != typeof(StringWriter))
        {
            // This overload was added after the WriteLineAsync(char[], ...) overload, and so in case
            // a derived type may have overridden it, we need to delegate to it, which the base does.
            return base.WriteLineAsync(value, cancellationToken);
        }

        if(cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        if(!_isOpen)
        {
            throw new ObjectDisposedException(null, "Cannot write to a closed TextWriter.");
        }

        _container.Append(value);
        WriteLine();
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(char[] buffer, int index, int count)
    {
        WriteLine(buffer, index, count);
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

    public override Task FlushAsync()
    {
        return Task.CompletedTask;
    }

    #endregion

    // Returns a string containing the characters written to this TextWriter so far.
    public override string ToString()
    {
        return _container.ToString();
    }
}
