namespace System.Text;

[StructLayout(LayoutKind.Auto)]
public struct IndentedStringBuilder
{
    private const byte IndentSize = 4;
    private byte _indent;
    private readonly byte _size;

    // 每一个append line都缩进，一行头一个append，其他不需要
    private bool _indentPending = true;

    private readonly StringBuilder _stringBuilder;

    public IndentedStringBuilder()
    {
        _stringBuilder = new();
        _size = IndentSize;
    }

    public IndentedStringBuilder(int capacity, byte size = IndentSize)
    {
        _stringBuilder = new(capacity);
        _size = size;
    }

    public int Length => _stringBuilder.Length;

    public IndentedStringBuilder AppendLines(string value, bool skipFinalNewline = false)
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

    public IndentedStringBuilder AppendLine()
    {
        AppendLine(string.Empty);

        return this;
    }

    public IndentedStringBuilder AppendLine(string value)
    {
        if(value.Length != 0)
        {
            DoIndent();
        }

        _stringBuilder.AppendLine(value);

        _indentPending = true;

        return this;
    }

    public IndentedStringBuilder Append(char value)
    {
        DoIndent();

        _stringBuilder.Append(value);

        return this;
    }

    public IndentedStringBuilder Append(string value)
    {
        DoIndent();

        _stringBuilder.Append(value);

        return this;
    }

    public IndentedStringBuilder Append(IEnumerable<string> values)
    {
        DoIndent();

        // AppendJoin也是使用的foreach
        foreach(var value in values)
        {
            _stringBuilder.Append(value);
        }

        return this;
    }

    public IndentedStringBuilder Append(IEnumerable<char> value)
    {
        DoIndent();

        foreach(var chr in value)
        {
            _stringBuilder.Append(chr);
        }

        return this;
    }

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

    public IndentedStringBuilder Clear()
    {
        _stringBuilder.Clear();

        _indent = 0;

        return this;
    }

    /// <summary>
    /// 缩进
    /// </summary>
    /// <returns></returns>
    public IDisposable Indent()
    {
        return new Indenter(this);
    }

    /// <summary>
    /// 暂停缩进（缩进长度设置为0）
    /// </summary>
    /// <returns></returns>
    public IDisposable SuspendIndent()
    {
        return new IndentSuspender(this);
    }

    private void DoIndent()
    {
        if(_indentPending && _indent > 0)
        {
            _stringBuilder.Append(' ', _indent * _size);
        }

        _indentPending = false;
    }

    /// <summary>
    /// 递增缩进长度
    /// </summary>
    public IndentedStringBuilder IncrementIndent()
    {
        _indent++;

        return this;
    }

    /// <summary>
    /// 递减缩进长度
    /// </summary>
    public IndentedStringBuilder DecrementIndent()
    {
        if(_indent > 0)
        {
            _indent--;
        }

        return this;
    }

    private struct IndentSuspender : IDisposable
    {
        private IndentedStringBuilder _stringBuilder;
        private readonly byte _indent;

        public IndentSuspender(IndentedStringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
            _indent = _stringBuilder._indent;
            _stringBuilder._indent = 0;
        }

        public void Dispose()
        {
            _stringBuilder._indent = _indent;
        }
    }

    private readonly struct Indenter : IDisposable
    {
        private readonly IndentedStringBuilder _stringBuilder;

        public Indenter(IndentedStringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;

            _stringBuilder.Increment();
        }

        public void Dispose()
        {
            _stringBuilder.Decrement();
        }
    }
}
