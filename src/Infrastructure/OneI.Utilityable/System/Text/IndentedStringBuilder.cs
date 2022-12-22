namespace System.Text;
/// <summary>
/// The indented string builder.
/// </summary>

public class IndentedStringBuilder
{
    /// <summary>
    /// The indent size.
    /// </summary>
    private const byte IndentSize = 4;
    private byte _indent;
    private readonly byte _size;

    // 每一个append line都缩进，一行头一个append，其他不需要
    private bool _indentPending = true;

    private readonly StringBuilder _stringBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndentedStringBuilder"/> class.
    /// </summary>
    public IndentedStringBuilder()
    {
        _stringBuilder = new();
        _size = IndentSize;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndentedStringBuilder"/> class.
    /// </summary>
    /// <param name="capacity">The capacity.</param>
    /// <param name="size">The size.</param>
    public IndentedStringBuilder(int capacity, byte size = IndentSize)
    {
        _stringBuilder = new(capacity);
        _size = size;
    }

    /// <summary>
    /// Gets the length.
    /// </summary>
    public virtual int Length => _stringBuilder.Length;

    /// <summary>
    /// 将给定字符串分隔为多行，然后将每行（以当前缩进为前缀，后跟新行）追加到正在生成的字符串中。
    /// </summary>
    /// <param name="value"></param>
    /// <param name="skipFinalNewline"></param>
    /// <returns></returns>
    public virtual IndentedStringBuilder AppendLines(string value, bool skipFinalNewline = false)
    {
        using(var reader = new StringReader(value))
        {
            var first = true;
            string? line;
            while((line = reader.ReadLine()) != null)
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    AppendLine();
                }

                if(line.Length != 0)
                {
                    Append(line);
                }
            }
        }

        if(!skipFinalNewline)
        {
            AppendLine();
        }

        return this;
    }

    /// <summary>
    /// Appends the line.
    /// </summary>
    /// <returns>An IndentedStringBuilder.</returns>
    public virtual IndentedStringBuilder AppendLine()
    {
        AppendLine(string.Empty);

        return this;
    }

    /// <summary>
    /// Appends the line.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>An IndentedStringBuilder.</returns>
    public virtual IndentedStringBuilder AppendLine(string value)
    {
        if(value.Length != 0)
        {
            DoIndent();
        }

        _stringBuilder.AppendLine(value);

        _indentPending = true;

        return this;
    }

    /// <summary>
    /// Appends the.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>An IndentedStringBuilder.</returns>
    public virtual IndentedStringBuilder Append(char value)
    {
        DoIndent();

        _stringBuilder.Append(value);

        return this;
    }

    /// <summary>
    /// Appends the.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>An IndentedStringBuilder.</returns>
    public virtual IndentedStringBuilder Append(string value)
    {
        DoIndent();

        _stringBuilder.Append(value);

        return this;
    }

    /// <summary>
    /// Appends the.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>An IndentedStringBuilder.</returns>
    public virtual IndentedStringBuilder Append(IEnumerable<string> values)
    {
        DoIndent();

        // AppendJoin也是使用的foreach
        foreach(var value in values)
        {
            _stringBuilder.Append(value);
        }

        return this;
    }

    /// <summary>
    /// Appends the.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>An IndentedStringBuilder.</returns>
    public virtual IndentedStringBuilder Append(IEnumerable<char> value)
    {
        DoIndent();

        foreach(var chr in value)
        {
            _stringBuilder.Append(chr);
        }

        return this;
    }

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        return _stringBuilder.ToString();
    }

    /// <summary>
    /// 递减缩进
    /// </summary>
    /// <returns></returns>
    private IndentedStringBuilder Increment()
    {
        _indent++;

        return this;
    }

    /// <summary>
    /// 递增缩进
    /// </summary>
    /// <returns></returns>
    private IndentedStringBuilder Decrement()
    {
        if(_indent > 0)
        {
            _indent--;
        }

        return this;
    }

    /// <summary>
    /// Clears the.
    /// </summary>
    /// <returns>An IndentedStringBuilder.</returns>
    public virtual IndentedStringBuilder Clear()
    {
        _stringBuilder.Clear();

        _indent = 0;

        return this;
    }

    /// <summary>
    /// 缩进
    /// </summary>
    /// <returns></returns>
    public virtual IDisposable Indent()
    {
        return new Indenter(this);
    }

    /// <summary>
    /// 暂停缩进（缩进长度设置为0）
    /// </summary>
    /// <returns></returns>
    public virtual IDisposable SuspendIndent()
    {
        return new IndentSuspender(this);
    }

    /// <summary>
    /// Dos the indent.
    /// </summary>
    private void DoIndent()
    {
        if(_indentPending && _indent > 0)
        {
            _stringBuilder.Append(' ', _indent * _size);
        }

        _indentPending = false;
    }

    /// <summary>
    /// Increments the indent.
    /// </summary>
    /// <returns>An IndentedStringBuilder.</returns>
    public virtual IndentedStringBuilder IncrementIndent()
    {
        _indent++;

        return this;
    }

    /// <summary>
    /// Decrements the indent.
    /// </summary>
    /// <returns>An IndentedStringBuilder.</returns>
    public virtual IndentedStringBuilder DecrementIndent()
    {
        if(_indent > 0)
        {
            _indent--;
        }

        return this;
    }

    /// <summary>
    /// The indent suspender.
    /// </summary>
    private sealed class IndentSuspender : IDisposable
    {
        private readonly IndentedStringBuilder _stringBuilder;
        private readonly byte _indent;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndentSuspender"/> class.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        public IndentSuspender(IndentedStringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
            _indent = _stringBuilder._indent;
            _stringBuilder._indent = 0;
        }

        /// <summary>
        /// Disposes the.
        /// </summary>
        public void Dispose()
        {
            _stringBuilder._indent = _indent;
        }
    }

    /// <summary>
    /// The indenter.
    /// </summary>
    private sealed class Indenter : IDisposable
    {
        private readonly IndentedStringBuilder _stringBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Indenter"/> class.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        public Indenter(IndentedStringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;

            _stringBuilder.Increment();
        }

        /// <summary>
        /// Disposes the.
        /// </summary>
        public void Dispose()
        {
            _stringBuilder.Decrement();
        }
    }
}
