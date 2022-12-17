namespace OneI.Textable.Templating.Properties;

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

    public void AddProperty(string name, PropertyValue property)
    {
        Properties.Add(name, property);
    }
}
