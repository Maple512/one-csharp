namespace OneI.Logable.Templating.Properties;

public class ObjectValue : PropertyValue
{
    public ObjectValue()
    {
        Properties = new();
    }

    public PropertyCollection Properties { get; }

    public override void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        writer.Write("{ ");
        var index = 0;
        foreach(var item in Properties.Keys)
        {
            writer.Write($"{item}: ");

            Properties[item].Render(writer, format, formatProvider);

            if(index++ != Properties.Count - 1)
            {
                writer.Write(", ");
            }
        }

        writer.Write(" }");
    }

    public void Add<T>(string name, T value, IFormatter<T>? formatter = null)
    {
        Properties.Add(name, CreateLiteral(value, formatter));
    }
}
