namespace OneI.Textable;

using OneI.Textable.Fakes;
using OneI.Textable.Templating.Properties;

public class PropertyValue_Test
{
    [Fact]
    public void literal_value()
    {
        PropertyValue.CreateLiteral(1)
            .ToString().ShouldBe("1");

        PropertyValue.CreateLiteral(new Model1() { Id = 1 })
            .ToString().ShouldBe("OneI.Textable.Fakes.Model1");

        PropertyValue.CreateLiteral(new Model1 { Id = 2 }, new ModelFormatter())
            .ToString().ShouldBe("Custom formatter");
    }

    [Fact]
    public void dictionary_value()
    {
        var value = new DictionaryValue();

        value.Add("key1", "value1");
        value.Add("key2", "value2");

        value.ToString().ShouldBe("""[{key1: value1}, {key2: value2}]""");
    }

    [Fact]
    public void enumerable_value()
    {
        var value = new EnumerableValue();

        value.Add(1);
        value.Add("123123");
        value.Add(1254.22m);

        value.ToString().ShouldBe("""[1, 123123, 1254.22]""");
    }

    [Fact]
    public void object_value()
    {
        var value = new ObjectValue();

        value.AddProperty("Id", 512);
        value.AddProperty("Name", "Maple512");

        value.ToString().ShouldBe("""{ Id: 512, Name: Maple512 }""");
    }
}
