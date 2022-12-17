namespace OneI.Textable;

using OneI.Textable.Templating.Properties;

public class Template_Test
{
    [Fact]
    public void text_parse()
    {
        var text = "[{Data:yyyy-MM-dd}] {1}";

        Template.Create(text)
            .With("Date", PropertyValue.CreateLiteral(DateTime.Now))
            .ToString()
            .ShouldBe("[2022-12-17]");
    }
}
