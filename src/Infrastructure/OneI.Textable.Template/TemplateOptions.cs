namespace OneI.Textable;

using OneI.Textable.Templating;
using OneI.Textable.Templating.Properties;
/// <summary>
/// The template options.
/// </summary>

public class TemplateOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateOptions"/> class.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    public TemplateOptions(IFormatProvider? formatProvider = null)
        : this(new(), formatProvider)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateOptions"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="formatProvider">The format provider.</param>
    public TemplateOptions(Dictionary<string, PropertyValue> values, IFormatProvider? formatProvider = null)
        : this(new(values), formatProvider)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateOptions"/> class.
    /// </summary>
    /// <param name="properties">The properties.</param>
    /// <param name="formatProvider">The format provider.</param>
    public TemplateOptions(PropertyCollection properties, IFormatProvider? formatProvider = null)
    {
        Properties = properties;
        FormatProvider = formatProvider;
    }

    /// <summary>
    /// Gets the properties.
    /// </summary>
    public PropertyCollection Properties { get; }

    /// <summary>
    /// Gets the format provider.
    /// </summary>
    public IFormatProvider? FormatProvider { get; }

    /// <summary>
    /// Adds the.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>A TemplateOptions.</returns>
    public TemplateOptions Add(string name, PropertyValue value)
    {
        Properties.Add(name, value);

        return this;
    }
}
