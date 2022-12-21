namespace OneI.Textable;

using OneI.Textable.Templating;
using OneI.Textable.Templating.Properties;

public class TemplateOptions
{
    public TemplateOptions(IFormatProvider? formatProvider = null)
        : this(new(), formatProvider)
    {
    }

    public TemplateOptions(Dictionary<string, PropertyValue> values, IFormatProvider? formatProvider = null)
        : this(new(values), formatProvider)
    {
    }

    public TemplateOptions(PropertyCollection properties, IFormatProvider? formatProvider = null)
    {
        Properties = properties;
        FormatProvider = formatProvider;
    }

    public PropertyCollection Properties { get; }

    public IFormatProvider? FormatProvider { get; }

    public TemplateOptions Add(string name, PropertyValue value)
    {
        Properties.Add(name, value);

        return this;
    }
}
