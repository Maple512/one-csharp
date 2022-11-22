namespace OneI.Logable.Rendering;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using OneI.Logable.Parsing;
using OneI.Logable.Properties;
using OneI.Logable.Properties.PropertyValues;

public static class TextRender
{
    private static readonly PropertyValueVisitor _visitor = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Render(
        IReadOnlyList<Token> tokens,
        IReadOnlyDictionary<string, PropertyValue> properties,
        TextWriter output,
        string? format = null,
        IFormatProvider? formatProvider = null)
    {
        var isJson = false;

        if(format.NotNullOrEmpty()
            && format.Length == 1)
        {
            isJson = format[0] == LoggerDefinition.Formatters.Json;
        }

        foreach(var token in tokens)
        {
            switch(token)
            {
                case PropertyToken pt:
                    PropertyTokenRender(pt, properties, output, isJson, formatProvider);
                    break;
                default:
                    TextTokenRender(token, output);
                    break;
            }
        }
    }

    public static void TextTokenRender(Token token, TextWriter output)
    {
        output.Write(token.Text);
    }

    public static void PropertyTokenRender(
        PropertyToken token,
        IReadOnlyDictionary<string, PropertyValue> properties,
        TextWriter output,
        bool isJson,
        IFormatProvider? formatProvider = null)
    {
        if(properties.TryGetValue(token.Name, out var propertyValue) == false)
        {
            output.Write(token.Text);

            return;
        }

        if(!token.Alignment.HasValue)
        {
            RenderValue(propertyValue, isJson, output, token.Format, formatProvider);
            return;
        }

        var valueOutput = new StringWriter();

        RenderValue(propertyValue, isJson, valueOutput, token.Format, formatProvider);
        var value = valueOutput.ToString();

        if(value.Length >= token.Alignment.Value.Width)
        {
            output.Write(value);

            return;
        }

        Padding.Apply(output, value, token.Alignment.Value);
    }

    private static void RenderValue(
        PropertyValue propertyValue,
        bool isJson,
        TextWriter output,
        string? format = null,
        IFormatProvider? formatProvider = null)
    {
        if(isJson == false
            && propertyValue is ScalarPropertyValue { Value: string str })
        {
            output.Write(str);
        }
        else if(isJson && format == null)
        {
            _visitor.Format(propertyValue, output);
        }
        else
        {
            propertyValue.Render(output, format, formatProvider);
        }
    }
}
