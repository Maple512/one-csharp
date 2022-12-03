namespace OneI.Logable.Templating.Rendering;

public static class Render
{
    public static void Padding(TextWriter writer, string value, Alignment? alignment)
    {
        if(alignment.HasValue is false
            || value is null
            || value.Length >= alignment.Value.Width)
        {
            writer.Write(value);

            return;
        }

        var align = alignment.Value;

        var pad = align.Width - value.Length;

        if(align.Direction == Direction.Left)
        {
            writer.Write(value);
        }

        writer.Write(new string(' ', pad));

        if(align.Direction == Direction.Right)
        {
            writer.Write(value);
        }
    }
}
