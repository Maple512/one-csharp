namespace OneI.Logable.Formatters;

//public class MessageFormatter : IPropertyValueFormatter<ReadOnlyMemory<char>>
//{
//    private readonly PropertyCollection _properties;

//    public MessageFormatter(PropertyCollection properties)
//    {
//        _properties = properties;
//    }

//    public void Format(string? value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
//    {
//        var template = new TemplateContext(_tokens, _properties.NamedProperties,_properties.IndexerProperties);

//        template.Render(writer, formatProvider);
//    }

//    public void Format(ReadOnlyMemory<char> value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
//    {
//        throw new NotImplementedException();
//    }
//}
