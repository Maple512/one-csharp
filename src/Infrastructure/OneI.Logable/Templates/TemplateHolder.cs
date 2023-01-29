namespace OneI.Logable.Templates;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// 模板占位符
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct TemplateHolder : IEquatable<TemplateHolder>
{
    public static readonly TemplateHolder Default = default;

    public readonly byte Indent;
    public readonly sbyte Align;
    public readonly sbyte ParameterIndex;
    public readonly PropertyType Type;
    public readonly ushort Position;
    public readonly ushort Length;
    public readonly string? Name;
    public readonly string? Format;

    public TemplateHolder()
    {
        ParameterIndex = -1;
    }

    private TemplateHolder(
        byte indent,
        sbyte align,
        sbyte parameterIndex,
        PropertyType type,
        ushort position,
        ushort length,
        string? name,
        string? format)
    {
        Indent = indent;
        Align = align;
        ParameterIndex = parameterIndex;
        Type = type;
        Position = position;
        Length = length;
        Name = name;
        Format = format;
    }

    internal static TemplateHolder CreateText(ushort position, ushort length)
    {
        return new TemplateHolder(0, 0, -1, default, position, length, null, null);
    }

    internal static TemplateHolder CreateNamed(
        byte indent,
        sbyte align,
        PropertyType type,
        ushort position,
        string? name,
        string? format)
    {
        return new TemplateHolder(indent, align, -1, type, position, 0, name, format);
    }

    internal static TemplateHolder CreateIndexer(
        byte indent,
        sbyte align,
        sbyte parameterIndex,
        PropertyType type,
        ushort position,
        string? format)
    {
        return new TemplateHolder(indent, align, parameterIndex, type, position, 0, null, format);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsText() => Length != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsIndexer() => ParameterIndex > -1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNamed() => ParameterIndex == -1;

    public override int GetHashCode()
    {
        return HashCode.Combine(Indent, Align, ParameterIndex, Type, Name, Format);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TemplateHolder th && Equals(th);
    }

    public override string ToString()
    {
        var handler = new DefaultInterpolatedStringHandler(9, 4, null, stackalloc char[30]);
        handler.AppendLiteral($"[{nameof(Position)}: ");
        handler.AppendFormatted(Position);
        handler.AppendLiteral(", ");
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
    {
        return Position == other.Position
            && Length == other.Length;
    }
}
