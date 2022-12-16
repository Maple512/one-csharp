namespace OneI.Logable.Templating;

using OneI.Logable.Templating.Properties;

public class PropertyValue_Test
{
    [Fact]
    public void key_value()
    {
        // 0123456789
        var value = new DictionaryValue();

        value.Add("key1", PropertyValue.CreateLiteral("value1"));
        value.Add("key2", PropertyValue.CreateLiteral("value2"));

        value.ToString().ShouldBe("""[{"key1": "value1"}, {"key2": "value2"}]""");
    }

    [Fact]
    public void enumerable_value()
    {
        var value = new EnumerableValue();

        value.AddPropertyValue(PropertyValue.CreateLiteral(1));
        value.AddPropertyValue(PropertyValue.CreateLiteral("123123"));
        value.AddPropertyValue(PropertyValue.CreateLiteral(1254.22m));

        value.ToString().ShouldBe("""[1, "123123", 1254.22]""");
    }

    [Fact]
    public void object_value()
    {
        var value = new ObjectValue();

        value.AddProperty("Id", PropertyValue.CreateLiteral(512));
        value.AddProperty("Name", PropertyValue.CreateLiteral("Maple512"));

        value.ToString().ShouldBe("""{ "Id": 512, "Name": "Maple512" }""");
    }
}
