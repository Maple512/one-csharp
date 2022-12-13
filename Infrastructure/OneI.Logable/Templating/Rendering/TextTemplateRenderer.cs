namespace OneI.Logable.Templating.Rendering;

using System.Globalization;
using System.IO;
using OneI.Logable.Templating.Formatting;
using PropertyNames = LoggerConstants.PropertyNames;

/// <summary>
/// 文本模版渲染器
/// </summary>
public class TextTemplateRenderer : ITextRenderer
{
    private readonly TextTemplate _outputTemplate;
    private readonly IFormatProvider? _formatProvider;

    public TextTemplateRenderer(string template, IFormatProvider? formatProvider = null)
    {
        _formatProvider = formatProvider;

        _outputTemplate = TextParser.Parse(template);
    }

    public void Render(LoggerContext context, TextWriter writer)
    {
        foreach(var token in _outputTemplate.Tokens)
        {
            switch(token)
            {
                case TextToken textToken:
                    RenderText(writer, textToken);
                    break;
                case PropertyToken propertyToken:
                    RenderProperty(writer, propertyToken, context);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown token type: {token.GetType().FullName}");
            }
        }
    }

    internal static void RenderText(TextWriter writer, TextToken token) => writer.Write(token.ToString());

    private void RenderProperty(TextWriter writer, PropertyToken token, LoggerContext context)
    {
        if(token.Name == PropertyNames.Level)
        {
            var formatted = LevelFormatter.Format(context.Level, token.Format);

            RenderHelper.Padding(writer, formatted, token.Alignment);
        }
        else if(token.Name == PropertyNames.NewLine)
        {
            RenderHelper.Padding(writer, Environment.NewLine, token.Alignment);
        }
        else if(token.Name == PropertyNames.Exception)
        {
            RenderHelper.Padding(writer, context.Exception?.ToString(), token.Alignment);
        }
        //else if(token.Name == PropertyNames.MemberName)
        //{
        //    RenderHelper.Padding(writer, context.MemberName, token.Alignment);
        //}
        //else if(token.Name == PropertyNames.FilePath)
        //{
        //    RenderHelper.Padding(writer, context.FilePath, token.Alignment);
        //}
        //else if(token.Name == PropertyNames.LineNumber)
        //{
        //    RenderHelper.Padding(writer, context.LineNumber?.ToString(), token.Alignment);
        //}
        else
        {
            // 在此块中，writer 可能用于缓冲输出，以便可以应用填充。
            var sw = token.Alignment.HasValue ? new StringWriter() : writer;

            if(token.Name == PropertyNames.Message)
            {
                MessageTemplateRender.Render(
                    context.MessageTemplate,
                    context.Properties,
                    sw,
                    token.Format,
                    _formatProvider);
            }
            else if(token.Name == PropertyNames.Timestamp)
            {
                DefaultRender(context.Timestamp, sw, token.Format, _formatProvider);
            }
            else if(token.Name == PropertyNames.Properties)
            {
                PropertiesFormatter.Format(
                    context.MessageTemplate,
                    context.Properties,
                    _outputTemplate,
                    sw,
                    token.Format,
                    _formatProvider);
            }
            else
            {
                // property
                if(context.Properties.TryGetValue(token.Name, out var value))
                {
                    value.Render(sw, token.Format, _formatProvider);
                }
            }

            if(token.Alignment.HasValue)
            {
                RenderHelper.Padding(writer, ((StringWriter)sw).ToString(), token.Alignment);
            }
        }
    }

    internal static void DefaultRender<T>(
        T? value,
        TextWriter writer,
        string? format = null,
        IFormatProvider? formatProvider = null)
    {
        if(value == null)
        {
            writer.Write("null");
            return;
        }

        if(value is string str)
        {
            if(format is not null
                and "l")
            {
                writer.Write(str);
            }
            else
            {
                writer.Write('"');
                writer.Write(str.Replace("\"", "\\\""));
                writer.Write("\"");
            }

            return;
        }

        var custom = (ICustomFormatter?)formatProvider?.GetFormat(typeof(ICustomFormatter));
        if(custom != null)
        {
            writer.Write(custom.Format(format, value, formatProvider));
            return;
        }

        if(value is IFormattable f)
        {
            writer.Write(f.ToString(format, formatProvider ?? CultureInfo.InvariantCulture));
        }
        else
        {
            writer.Write(value.ToString());
        }
    }
}
