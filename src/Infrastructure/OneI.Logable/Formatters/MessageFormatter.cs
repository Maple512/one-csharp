namespace OneI.Logable.Formatters;

using System;
using System.Collections.Generic;
using OneI.Logable.Templatizations;
using OneI.Logable.Templatizations.Tokenizations;

public class MessageFormatter : IPropertyValueFormatter<string>
{
    private readonly IReadOnlyList<ITemplateToken> _tokens;
    private readonly PropertyCollection _properties;

    public MessageFormatter(IReadOnlyList<ITemplateToken> tokens, PropertyCollection properties)
    {
        _tokens = tokens;
        _properties = properties;
    }

    public void Format(in string? value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        var template = new TemplateContext(_tokens, _properties.ToList());

        template.Render(writer, formatProvider);
    }
}
