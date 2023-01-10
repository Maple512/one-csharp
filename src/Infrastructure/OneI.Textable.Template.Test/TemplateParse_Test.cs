namespace OneI.Textable;

using OneI.Textable.Templating;

public class TemplateParse_Test
{
    [Fact]
    public void parse_text()
    {
        var text = "{Date:yyyy-MM-dd HH:mm:ss,-12'100}";

        var tokens = TemplateParser.Parse(text);

        tokens.Count().ShouldBe(1);

        tokens.First().ShouldBeAssignableTo<PropertyToken>();
    }
}
