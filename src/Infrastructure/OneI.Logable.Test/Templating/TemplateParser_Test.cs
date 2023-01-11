namespace OneI.Logable.Templating;

public class TemplateParser_Test
{
    [Fact]
    public void parse_text()
    {
        var text = "{Date:yyyy-MM-dd HH:mm:ss,-12'10}";

        var tokens = TemplateParser.Parse(text);

        tokens.Count.ShouldBe(1);

        tokens.First().ShouldBeAssignableTo<PropertyToken>();
    }
}
