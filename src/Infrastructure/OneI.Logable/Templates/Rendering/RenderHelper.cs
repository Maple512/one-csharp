namespace OneI.Textable.Rendering;

using System.Globalization;
using OneI.Textable;

internal static class RenderHelper
{
    public static void Padding(TextWriter writer, string? value, Alignment? alignment)
    {
        if(value.IsNullOrWhiteSpace())
        {
            writer.Write(value);
            return;
        }

        if(alignment.HasValue is false
            || value.Length >= alignment.Value.Width)
        {
            writer.Write(value);

            return;
        }

        var pad = alignment.Value.Width - value.Length;

        if(alignment.Value.Direction == Direction.Left)
        {
            writer.Write(value);
        }

        writer.Write(new string(' ', pad));

        if(alignment.Value.Direction == Direction.Right)
        {
            writer.Write(value);
        }
    }

    public static void Render<T>(
        T? value,
        IFormatter<T>? formatter,
        TextWriter writer,
        string? format = null,
        IFormatProvider? formatProvider = null)
    {
        if(formatter != null)
        {
            formatter.Format(value, writer, format, formatProvider);
            return;
        }

        if(value == null)
        {
            writer.Write("null");
            return;
        }

        if(value is string str)
        {
            writer.Write(str);

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
