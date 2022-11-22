namespace OneI.Logable.Rendering;

using System.IO;

public static class Padding
{
    public static void Apply(TextWriter output, string value, Alignment? alignment)
    {
        if(alignment == null
            || value.Length >= alignment.Value.Width)
        {
            output.Write(value);
            return;
        }

        var pad = alignment.Value.Width - value.Length;

        if(alignment.Value.Direction == Direction.Left)
        {
            output.Write(value);
        }

        output.Write(new string(' ', pad));

        if(alignment.Value.Direction == Direction.Right)
        {
            output.Write(value);
        }
    }
}
