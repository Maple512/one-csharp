namespace System.Text;

internal class IndentedStringBuilder
{
    private const byte IndentSize = 4;

    private readonly byte _size;

    private readonly StringBuilder _stringBuilder;
    private byte _indent;

    // 每一个append line都缩进，一行头一个append，其他不需要
    private bool _indentPending = true;

    public IndentedStringBuilder()
    {
        _stringBuilder = new StringBuilder();
        _size = IndentSize;
    }

    public IndentedStringBuilder(int capacity, byte size = IndentSize)
    {
        _stringBuilder = new StringBuilder(capacity);
        _size = size;
    }

    public int Length
    {
        get
        {
            return _stringBuilder.Length;
        }
    }

    /// <summary>
    ///     将给定字符串分隔为多行，然后将每行（以当前缩进为前缀，后跟新行）追加到正在生成的字符串中。
    /// </summary>
    /// <param name="value"></param>
    /// <param name="skipFinalNewline"></param>
    /// <returns></returns>
    public void AppendLines(string value, bool skipFinalNewline = false)
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
    }

    public void AppendLine()
    {
        _ = _stringBuilder.AppendLine();
    }

    public void AppendLine(string value)
    {
        if(value.Length != 0)
        {
            DoIndent();
        }

        _ = _stringBuilder.AppendLine(value);

        _indentPending = true;
    }

    public void Append(char value)
    {
        DoIndent();

        _ = _stringBuilder.Append(value);
    }

    public void Append(string value)
    {
        DoIndent();

        _ = _stringBuilder.Append(value);
    }

    public void Append(IEnumerable<string> values)
    {
        DoIndent();

        // AppendJoin也是使用的foreach
        foreach(var value in values)
        {
            _ = _stringBuilder.Append(value);
        }
    }

    public void Append(IEnumerable<char> value)
    {
        DoIndent();

        foreach(var chr in value)
        {
            _ = _stringBuilder.Append(chr);
        }
    }

    public override string ToString() => _stringBuilder.ToString();

    private void Increment()
    {
        _indent++;
    }

    private void Decrement()
    {
        if(_indent > 0)
        {
            _indent--;
        }
    }

    public void Clear()
    {
        _ = _stringBuilder.Clear();

        _indent = 0;
    }

    public IDisposable Indent() => new Indenter(this);

    public IDisposable DeIndent() => new DeIndenter(this);

    /// <summary>
    ///     暂停缩进（缩进长度设置为0）
    /// </summary>
    /// <returns></returns>
    public IDisposable SuspendIndent() => new IndentSuspender(this);

    private void DoIndent()
    {
        if(_indentPending && _indent > 0)
        {
            _ = _stringBuilder.Append(' ', _indent * _size);
        }

        _indentPending = false;
    }

    private sealed class IndentSuspender : IDisposable
    {
        private readonly byte _indent;
        private readonly IndentedStringBuilder _stringBuilder;

        public IndentSuspender(IndentedStringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
            _indent = _stringBuilder._indent;
            _stringBuilder._indent = 0;
        }

        public void Dispose() => _stringBuilder._indent = _indent;
    }

    private sealed class Indenter : IDisposable
    {
        private readonly IndentedStringBuilder _stringBuilder;

        public Indenter(IndentedStringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;

            _stringBuilder.Increment();
        }

        public void Dispose() => _stringBuilder.Decrement();
    }

    private sealed class DeIndenter : IDisposable
    {
        private readonly IndentedStringBuilder _stringBuilder;

        public DeIndenter(IndentedStringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;

            _stringBuilder.Decrement();
        }

        public void Dispose() => _stringBuilder.Increment();
    }
}
