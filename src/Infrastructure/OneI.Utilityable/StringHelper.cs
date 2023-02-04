namespace OneI;

using OneI.Text;

public class StringHelper
{
    public static string EscapeString(scoped ReadOnlySpan<char> chars)
    {
        var buffer = new RefValueStringBuilder(stackalloc char[chars.Length]);

        // TODO: 等net8
        //chars.IndexOfAny()

        return chars.ToString();
    }
}
