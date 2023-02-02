namespace OneI.Logable.Templates;

/// <summary>
///     模板占位符
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct TemplateHolder : IEquatable<TemplateHolder>
{
    public readonly byte Indent;
    public readonly sbyte Align;
    public readonly sbyte ParameterIndex;
    public readonly PropertyType Type;
    public readonly ushort Length;
    public readonly ReadOnlyMemory<char> Text;
    public readonly string? Name;
    public readonly string Format;

#pragma warning disable CS8618 
    public TemplateHolder() => ParameterIndex = -1;
#pragma warning restore CS8618

    private TemplateHolder(byte indent
                           , sbyte align
                           , sbyte parameterIndex
                           , PropertyType type
                           , ushort length
                           , string? name
                           , string format
                           , ReadOnlyMemory<char> text)
    {
        Indent = indent;
        Align = align;
        ParameterIndex = parameterIndex;
        Type = type;
        Length = length;
        Name = name;
        Format = format;
        Text = text;
    }

    internal static TemplateHolder CreateText(ReadOnlyMemory<char> text)
        => new(0, 0, -1, default, (ushort)text.Length, null, null!, text);

    internal static TemplateHolder CreateNamed(byte indent
                                               , sbyte align
                                               , PropertyType type
                                               , string? name
                                               , string format)
        => new(indent, align, -1, type, ushort.MinValue, name, format, null);

    internal static TemplateHolder CreateIndexer(byte indent
                                                 , sbyte align
                                                 , sbyte parameterIndex
                                                 , PropertyType type
                                                 , string format)
        => new(indent, align, parameterIndex, type, ushort.MinValue, null, format, null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsText() => Length != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsIndexer() => ParameterIndex > -1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNamed() => ParameterIndex == -1;

    public override int GetHashCode() => HashCode.Combine(Indent, Align, ParameterIndex, Type, Name, Format);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is TemplateHolder th && Equals(th);

    public override string ToString()
    {
        var handler = new DefaultInterpolatedStringHandler(9, 3, null, stackalloc char[30]);
        handler.AppendLiteral($"[");
        if(IsText())
        {
            handler.AppendLiteral($"{nameof(Length)}: ");
            handler.AppendFormatted(Length);

            handler.AppendLiteral(", Text");
        }
        else if(IsIndexer())
        {
            handler.AppendLiteral($"{nameof(ParameterIndex)}: ");
            handler.AppendFormatted(ParameterIndex);
            handler.AppendLiteral(", Indexer");
        }
        else
        {
            handler.AppendLiteral($"{nameof(Name)}: ");
            handler.AppendFormatted(Name);
            handler.AppendLiteral(", Named");
        }

        handler.AppendLiteral("]");

        return handler.ToStringAndClear();
    }

    public bool Equals(TemplateHolder other)
        => ParameterIndex == other.ParameterIndex
           || Text.Equals(other.Text);

    public static bool operator ==(TemplateHolder left, TemplateHolder right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TemplateHolder left, TemplateHolder right)
    {
        return !(left == right);
    }
}
