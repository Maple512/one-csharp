namespace OneI.Logable.Templatizations;

using System.Globalization;
using OneI.Logable.Templatizations.Tokenizations;

public static class TemplateRenderHelper
{
    public static void Render<TValue>(
        TValue? value,
        TextWriter writer,
        PropertyTokenType type,
        string? format,
        IPropertyValueFormatter<TValue>? formatter,
        IFormatProvider? formatProvider)
    {
        if(value == null)
        {
            writer.Write("[Null]");
            return;
        }

        if(formatter != null)
        {
            formatter.Format(value, writer, format, formatProvider);
            return;
        }

        if(value is string str)
        {
            if(type == PropertyTokenType.Stringify)
            {
                writer.Write("\"");
                writer.Write(str.Replace("\"", "\\\""));
                writer.Write("\"");
            }
            else
            {
                writer.Write(str);
            }

            return;
        }

        var custom = (ICustomFormatter?)formatProvider?.GetFormat(typeof(ICustomFormatter));
        if(custom != null)
        {
            writer.Write(custom.Format(format, value, formatProvider));
            return;
        }

        if(value is IFormattable f)
        {
            writer.Write(f.ToString(format, formatProvider ?? CultureInfo.InvariantCulture));
            return;
        }

        writer.Write(value.ToString());
    }
}
