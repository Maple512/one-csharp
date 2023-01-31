namespace OneI.Logable;

using OneI.Logable.Templates;

public class LoggerProperty_Test
{
    [Fact]
    public void create_property()
    {
        var value = Randomizer.String(100);

        var property = new PropertyValue(value);

        property.ToString().ShouldBe(value);
    }
}
