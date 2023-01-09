namespace OneI.Textable;

using OneI.Buffers;
using OneI.Textable.Rendering;
using OneI.Textable.Templating;
using System.Globalization;

public class TemplateParse_Test
{
    [Fact]
    public void parse_text()
    {
        var text = "{Date:yyyy-MM-dd HH:mm:ss,-12'100}";

        var buffer = new ValueBuffer<char>(text.Length);

        text.CopyTo(buffer);

        var tokens = ParseCore(buffer);

        tokens.Count().ShouldBe(1);

        tokens.First().ShouldBeAssignableTo<PropertyToken>();
    }

    private static IEnumerable<Token> ParseCore(scoped in ValueBuffer<char> text)
    {
        if(text.IsEmpty)
        {
            return Enumerable.Empty<Token>();
        }

        var result = new List<Token>();

        var index = 0;
        var lastIndex = text.Length - 1;

        var openBrace = -1;
        var closeBrace = -1;
        var propertyIndex = 0;
        var propertyTotalLength = 0;

        const int IndexLimit = 1000;
        const int WidthLimit = 1000;
        const int IndentLimit = 1000;

        // current position
        var pos = 0;

        while(true)
        {
            if(pos >= text.Length)
            {
                break;
            }

            var remainder = text[pos..];
            var ob = remainder.IndexOf('{');
            if(ob == -1)
            {
                result.Add(new TextToken(pos, remainder.ToString()));
                break;
            }

            var cb = remainder.IndexOf('}');
            if(cb > ob + 1)
            {
                // {Date:yyyy-MM-dd HH:mm:ss,-12'100}
                // like property token

                var formatStartIndex = remainder.IndexOf(':');
                var alignStartIndex = remainder.IndexOf(',');
                var indentStartIndex = remainder.IndexOf('\'');


            }
        }

        return result;
    }

    /// <summary>
    /// Tries the valid property name.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="name">The name.</param>
    /// <returns>A bool.</returns>
    private static bool TryValidPropertyName(ReadOnlySpan<char> text, [NotNullWhen(true)] out string? name)
    {
        name = null;

        for(var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if(char.IsLetterOrDigit(c) == false
                && c != '_')
            {
                return false;
            }
        }

        name = text.ToString();

        return true;
    }

    /// <summary>
    /// Tries the valid indent.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="indent">The indent.</param>
    /// <returns>A bool.</returns>
    private static bool TryValidIndent(ReadOnlySpan<char> text, out int? indent)
    {
        indent = 0;
        if(text.IsEmpty || text.IsWhiteSpace())
        {
            return false;
        }

        int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var result);

        indent = result;

        return indent != 0;
    }

    /// <summary>
    /// Tries the valid align.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="align">The align.</param>
    /// <returns>A bool.</returns>
    private static bool TryValidAlign(ReadOnlySpan<char> text, [NotNullWhen(true)] out Alignment? align)
    {
        align = null;
        if(text.IsEmpty || text.IsWhiteSpace())
        {
            return false;
        }

        if(int.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var width))
        {
            align = new Alignment(width);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries the valid format.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="format">The format.</param>
    /// <returns>A bool.</returns>
    private static bool TryValidFormat(ReadOnlySpan<char> text, [NotNullWhen(true)] out string? format)
    {
        format = null;
        if(text.IsEmpty || text.IsWhiteSpace())
        {
            return false;
        }

        foreach(var item in text)
        {
            if(IsValidFormat(item) == false)
            {
                return false;
            }
        }

        format = text.ToString();

        return true;
    }

    /// <summary>
    /// Are the valid format.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <returns>A bool.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidFormat(char c)
    {
        return char.IsLetterOrDigit(c) // 字母或数字
            || char.IsPunctuation(c) // 标点符号
            || char.IsWhiteSpace(c) // 空格
            || char.IsSymbol(c); // 字符
    }
}
