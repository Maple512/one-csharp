namespace OneI.Logable.Templating;

using OneI.Logable.Rendering;

public class TextParser_Test
{
    [Theory]
    [InlineData("{Data}", "Data", null, null)]
    [InlineData("{Data,-56}", "Data", null, -56)]
    [InlineData("{Data:yyyy-MM-dd}", "Data", "yyyy-MM-dd", null)]
    [InlineData("{Data:yyyy-MM-dd,23}", "Data", "yyyy-MM-dd", 23)]
    [InlineData("{Data,-56:yyyy-MM-dd}", "Data", "yyyy-MM-dd", -56)]
    public void parse_property_token(string text, string name, string format, int? width)
    {
        var tokens = TextParser.Parse(text).Tokens;

        tokens.Count.ShouldBe(1);

        var t = tokens[0] as PropertyToken;

        t.ShouldNotBeNull();

        t.Name.ShouldBe(name);
        t.Format.ShouldBe(format);

        if(width.HasValue)
        {
            t.Alignment!.Value.ShouldBe(new Alignment(width.Value));
        }
        else
        {
            t.Alignment.HasValue.ShouldBeFalse();
        }
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{{}")]
    [InlineData("{{}{}{}{}}}}}}")]
    public void parse_text_token(string text)
    {
        var tokens = TextParser.Parse(text).Tokens;

        tokens.OfType<TextToken>().Count().ShouldBe(1);
    }

    [Theory]
    [InlineData("[{Timestamp:yyyy-MM-dd} {Level}] {Message:j}{NewLine}{Exception}", 3, 5)]
    [InlineData("The assembly is {Assembly}", 1, 1)]
    [InlineData("The assembly is {Assembly}{Date::yyyy年}", 1, 2)]
    [InlineData("The assembly is {Assembly}{Date:yyyy年}", 1, 2)]
    public void mixed_tokens(string template, int textCount, int propertyCount)
    {
        var tokens = TextParser.Parse(template).Tokens;

        tokens.OfType<TextToken>().Count().ShouldBe(textCount);
        tokens.OfType<PropertyToken>().Count().ShouldBe(propertyCount);
    }

    [Theory]
    [InlineData("{Exception,,}")]
    [InlineData("{Exception::}", false)]
    [InlineData("{:Exception}")]
    [InlineData("{,Exception}")]
    [InlineData("{,:Exception}")]
    [InlineData("{:,Exception}")]
    [InlineData("{:}")]
    [InlineData("{,}")]
    [InlineData("{Exception:yyyy,}")]
    public void invalid_property_token(string template, bool isText = true)
    {
        var tokens = TextParser.Parse(template).Tokens;

        if(isText)
        {
            tokens.OfType<TextToken>().Count().ShouldBe(1);
        }
        else
        {
            tokens.OfType<PropertyToken>().Count().ShouldBe(1);
        }
    }
}
