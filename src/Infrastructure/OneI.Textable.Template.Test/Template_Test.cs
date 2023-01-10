namespace OneI.Textable;

using OneI.Textable.Rendering;
using OneI.Textable.Templating;

public class Template_Test
{
    [Fact]
    public void text_parse()
    {
        TextTemplate.Create("{Date:yyyy-MM-dd,5}[{0:yyyy}]").AddProperty("Date", new DateTime(2022, 12, 17))
        .ToString().ShouldBe("2022-12-17[2022]");

        TextTemplate.Create("{Date:yyyy-MM-dd,20}[{0:yyyy}]").AddProperty("Date", new DateTime(2022, 12, 17))
            .ToString().ShouldBe("          2022-12-17[2022]");

        TextTemplate.Create("{Date:yyyy-MM-dd,-20}[{0:yyyy}]").AddProperty("Date", new DateTime(2022, 12, 17))
            .ToString().ShouldBe("2022-12-17          [2022]");
    }

    [Fact]
    public void full_token()
    {
        var text = "{Date:yyyy-MM-dd,20'12}";

        var template = TextTemplate.Create(text);
        var token = (template.Tokens[0] as PropertyToken)!;

        token.Text.ShouldBe("Date:yyyy-MM-dd,20'12");
        token.Format.ShouldBe("yyyy-MM-dd");
        token.Alignment!.Value.Width.ShouldBe(20);
        token.Alignment!.Value.Direction.ShouldBe(Direction.Right);
        token.Indent.ShouldBe(12);
        token.Index.ShouldBe(0);
        token.Name.ShouldBe("Date");
        token.Position.ShouldBe(0);
        token.ParameterIndex.ShouldBeNull();
    }

    [Theory]
    [InlineData("{Data'23}", "Data", null, null, 23)]
    [InlineData("{Data,-56}", "Data", null, -56)]
    [InlineData("{Data:yyyy-MM-dd}", "Data", "yyyy-MM-dd", null)]
    [InlineData("{Data:yyyy-MM-dd,23}", "Data", "yyyy-MM-dd", 23)]
    [InlineData("{Data,-56:yyyy-MM-dd'23}", "Data", "yyyy-MM-dd", -56, 23)]
    public void parse_property_token(string text, string name, string? format = null, int? width = null, int? indent = null)
    {
        var tokens = TextTemplate.Create(text).Tokens;

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

        t.Indent.ShouldBe(indent);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{{}")]
    [InlineData("{{}{}{}{}}}}}}")]
    public void parse_text_token(string text)
    {
        var tokens = TextTemplate.Create(text).Tokens;

        tokens.OfType<TextToken>().Count().ShouldBe(1);
    }

    [Theory]
    [InlineData("[{Timestamp:yyyy-MM-dd} {Level}] {Message:j}{NewLine}{Exception}", 3, 5)]
    [InlineData("The assembly is {Assembly}", 1, 1)]
    [InlineData("The assembly is {Assembly}{Date::yyyy年}", 1, 2)]
    [InlineData("The assembly is {Assembly}{Date:yyyy年}", 1, 2)]
    [InlineData("The assembly is {Assembly}{Date:yyyy年'-34}", 2, 1)]
    public void mixed_tokens(string template, int textCount, int propertyCount)
    {
        var tokens = TextTemplate.Create(template).Tokens;

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
        var tokens = TextTemplate.Create(template).Tokens;

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
