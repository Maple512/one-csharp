namespace OneI.Logable.Templating.Properties.ValueTypes;

public class DictionaryValue_Test
{
    [Fact]
    public void key_value_render()
    {
        var value = new DictionaryValue();

        value.Add("key1", PropertyValue.CreateLiteral("value1"));
        value.Add("key2", PropertyValue.CreateLiteral("value2"));

        value.ToString().ShouldBe("""[{"key1": "value1"}, {"key2": "value2"}]""");
    }
}