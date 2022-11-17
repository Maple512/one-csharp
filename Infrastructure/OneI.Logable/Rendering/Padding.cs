namespace OneI.Logable.Rendering;

using System.IO;

public static class Padding
{
    public static void Apply(TextWriter output, string value, Alignment alignment)
    {
        if(alignment.Direction == Direction.None
            || value.Length >= alignment.Width)
        {
            output.Write(value);
            return;
        }

        var pad = alignment.Width - value.Length;

        if(alignment.Direction == Direction.Left)
        {
            output.Write(value);
        }

        output.Write(new string(' ', pad));

        if(alignment.Direction == Direction.Right)
        {
            output.Write(value);
        }
    }
}
