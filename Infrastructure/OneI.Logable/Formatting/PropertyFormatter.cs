namespace OneI.Logable.Formatting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using OneI.Logable.Parsing;
using OneI.Logable.Properties;
using OneI.Logable.Properties.PropertyValues;

public static class PropertyFormatter
{
    private static readonly PropertyValueVisitor _propertyValueVisitor = new();

    public static void Render(
        IReadOnlyList<PropertyToken> tokens,
        IReadOnlyDictionary<string, PropertyValue> properties,
        IReadOnlyList<PropertyToken> outputTokens,
        TextWriter output,
        string? format,
        IFormatProvider? formatProvider)
    {
        if(format?.Length == 1
            && format[0] == LoggerDefinition.Formatters.Json)
        {
            // TODO: ???，为什么是都不包含
            var structureProperties = properties
                .Where(x => !ContainsPropertyName(tokens, x.Key)
                                                                && !ContainsPropertyName(outputTokens, x.Key))
                .Select(x => new Property(x.Key, x.Value));

            var value = new StructurePropertyValue(structureProperties);

            _propertyValueVisitor.Format(value, output);

            return;
        }

        output.Write("{ ");
        var delim = string.Empty;
        foreach(var property in properties)
        {
            if(ContainsPropertyName(tokens, property.Key)
                || ContainsPropertyName(outputTokens, property.Key))
            {
                continue;
            }

            output.Write(delim);
            delim = ", ";
            output.Write(property.Key);
            output.Write(": ");
            property.Value.Render(output, null, formatProvider);
        }

        output.Write(" }");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ContainsPropertyName(IReadOnlyList<PropertyToken> tokens, string propertyName)
        => tokens.Any(x => x.Name == propertyName);
}
