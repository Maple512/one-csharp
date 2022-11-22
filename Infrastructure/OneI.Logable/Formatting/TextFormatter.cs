namespace OneI.Logable.Formatting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OneI.Logable.Parsing;
using OneI.Logable.Properties.PropertyValues;
using OneI.Logable.Rendering;

public class TextFormatter : ITextFormatter
{
    private readonly IFormatProvider? _formatProvider;
    private readonly IReadOnlyList<Token> _tokens;

    public TextFormatter(string template, IFormatProvider? formatProvider = null)
    {
        _formatProvider = formatProvider;

        _tokens = TextParser.Parse(template);
    }

    public TextFormatter(IReadOnlyList<Token> tokens, IFormatProvider? formatProvider = null)
    {
        _formatProvider = formatProvider;
        _tokens = tokens;
    }

    public void Format(LoggerContext context, TextWriter output)
    {
        foreach(var token in _tokens)
        {
            if(token is TextToken tt)
            {
                TextRender.TextTokenRender(tt, output);

                continue;
            }

            var propertyToken = (PropertyToken)token;

            if(propertyToken.Name == LoggerDefinition.PropertyNames.Level)
            {
                var level = LevelFormatter.GetLevelMoniker(context.Level, propertyToken.Format);
                Padding.Apply(output, level, propertyToken.Alignment);
            }
            else if(propertyToken.Name == LoggerDefinition.PropertyNames.Member)
            {
                Padding.Apply(output, context.MemberName!, propertyToken.Alignment);
            }
            else if(propertyToken.Name == LoggerDefinition.PropertyNames.Line)
            {
                Padding.Apply(output, context.LineNumber?.ToString()!, propertyToken.Alignment);
            }
            else if(propertyToken.Name == LoggerDefinition.PropertyNames.File)
            {
                Padding.Apply(output, context.FilePath!, propertyToken.Alignment);
            }
            else if(propertyToken.Name == LoggerDefinition.PropertyNames.NewLine)
            {
                Padding.Apply(output, Environment.NewLine, propertyToken.Alignment);
            }
            else if(propertyToken.Name == LoggerDefinition.PropertyNames.Exception)
            {
                var exception = context.Exception == null ? string.Empty : $"{context.Exception}{Environment.NewLine}";

                Padding.Apply(output, exception, propertyToken.Alignment);
            }
            else
            {
                var writer = propertyToken.Alignment.HasValue
                    ? new StringWriter()
                    : output;

                if(propertyToken.Name == LoggerDefinition.PropertyNames.Message)
                {
                    TextRender.Render(
                        context.Tokens,
                        context.Properties,
                        writer,
                        propertyToken.Format,
                        _formatProvider);
                }
                else if(propertyToken.Name == LoggerDefinition.PropertyNames.Timestamp)
                {
                    ScalarPropertyValue.Render(
                        context.Timestamp,
                        writer,
                        propertyToken.Format,
                        _formatProvider);
                }
                else if(propertyToken.Name == LoggerDefinition.PropertyNames.Properties)
                {
                    PropertyFormatter.Render(
                        context.Tokens.OfType<PropertyToken>().ToArray(),
                        context.Properties,
                        _tokens.OfType<PropertyToken>().ToArray(),
                        writer,
                        propertyToken.Format,
                        _formatProvider);
                }
                else
                {
                    if(context.Properties.TryGetValue(propertyToken.Name, out var propertyValue) == false)
                    {
                        continue;
                    }

                    if(propertyValue is ScalarPropertyValue and { Value: string literal })
                    {
                        var cased = Casing.Format(literal, propertyToken.Format);
                        writer.Write(cased);
                    }
                    else
                    {
                        propertyValue.Render(writer, propertyToken.Format, _formatProvider);
                    }
                }

                if(propertyToken.Alignment.HasValue)
                {
                    Padding.Apply(output, writer.ToString()!, propertyToken.Alignment);
                }
            }
        }
    }
}
