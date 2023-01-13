namespace OneI.Logable.Templatizations;

using System;
using System.ComponentModel;
using Tokenizations;

public class TemplateContext
{
    public TemplateContext(
        IReadOnlyList<ITemplateToken> tokens,
        IReadOnlyList<ITemplateProperty> properties)
    {
        Tokens = tokens;
        Properties = properties;
    }

    public IReadOnlyList<ITemplateToken> Tokens { get; }

    public IReadOnlyList<ITemplateProperty> Properties { get; }

    public void Render(TextWriter writer, IFormatProvider? formatProvider = null)
    {
        foreach(var token in Tokens)
        {
            switch(token)
            {
                case TextToken tt:
                    TextRender(writer, tt);
                    break;
                case IndexerPropertyToken indexer:
                    ITemplateProperty? p1 = null;
                    if(Properties.Count > indexer.ParameterIndex)
                    {
                        p1 = Properties[indexer.ParameterIndex];
                    }

                    PropertyValueRender(writer, indexer, p1?.Value, formatProvider);
                    break;
                case NamedPropertyToken named:
                    var p2 = Properties.OfType<NamedProperty>()
                            .FirstOrDefault(x => x.Name.Equals(named.Name, StringComparison.OrdinalIgnoreCase));

                    PropertyValueRender(writer, named, p2?.Value, formatProvider);
                    break;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void TextRender(TextWriter writer, TextToken token) => writer.Write(token.Text);

    public static void PropertyValueRender(
        TextWriter writer,
        ITemplatePropertyToken token,
        ITemplatePropertyValue? value,
        IFormatProvider? formatProvider)
    {
        if(value == null)
        {
            writer.Write("[Null]");
        }
        else
        {
            if(token.Indent is not null)
            {
                WriteWhiteSpace(writer, token.Indent.Value);
            }

            var w2 = token.Alignment.HasValue ? new StringWriter() : writer;

            value.Render(writer, token.Type, token.Format, formatProvider);

            if(token.Alignment.HasValue)
            {
                var align = token.Alignment.Value;

                var valueStr = ((StringWriter)w2).ToString();

                if(valueStr.Length >= align.Width)
                {
                    writer.Write(valueStr);
                    return;
                }

                var pad = align.Width - valueStr.Length;

                if(align.Direction == TextDirection.Left)
                {
                    writer.Write(valueStr);
                }

                WriteWhiteSpace(writer, pad);

                if(align.Direction == TextDirection.Right)
                {
                    writer.Write(valueStr);
                }
            }
        }
    }

    private static void WriteWhiteSpace(TextWriter writer, int length)
    {
        Span<char> span = stackalloc char[length];
        span.Fill(' ');
        writer.Write(span);
        span.Clear();
    }

    public string ToString(IFormatProvider? formatProvider)
    {
        var writer = new StringWriter();

        Render(writer, formatProvider);

        return writer.ToString();
    }

    [Obsolete("Please do not use this method, it will always return 0.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS0809 // 过时成员重写未过时成员
    public override int GetHashCode() => 0;
#pragma warning restore CS0809 // 过时成员重写未过时成员

    public override string ToString() => ToString(null);
}
