namespace OneI.Logable.Templating.Properties.ValueTypes;

using System.Text.Json;

public class SequenceValue_Test
{
    [Fact]
    public void string_values_render()
    {
        var dic = new Dictionary<string, string>()
        {
            {"1","2" }, {"2","3"}, {"3","4"},
        };

        var s = JsonSerializer.Serialize(dic, JsonSerializationTools.StandardInstance());

        var result = Render(values: new[]
        {
            PropertyValue.Create(1),
            PropertyValue.Create("123123"),
            PropertyValue.Create(1254.22m),
        });

        result.ShouldBe("""[1, "123123", 1254.22]""");
    }

    private static string Render(string? format = null, IFormatProvider? formatProvider = null, params PropertyValue[] values)
    {
        var writer = new StringWriter();

        var value = new EnumerableValue();

        value.AddPropertyValues(values);

        value.Render(writer, format, formatProvider);

        return writer.ToString();
    }
}
