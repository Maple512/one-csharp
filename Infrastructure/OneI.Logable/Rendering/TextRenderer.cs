//namespace OneI.Logable.Rendering;

//using System.IO;
//using OneI.Logable.Formatting;
//using OneI.Textable;
//using OneI.Textable.Templating;
//using PropertyNames = LoggerConstants.PropertyNames;

//public class TextRenderer : ILoggerRenderer
//{
//    private readonly TemplateContext _outputTemplate;
//    private readonly IFormatProvider? _formatProvider;

//    public TextRenderer(string template, IFormatProvider? formatProvider = null)
//    {
//        _formatProvider = formatProvider;

//        _outputTemplate = TemplateParser.Parse(template);
//    }

//    public void Render(LoggerContext context, TextWriter writer)
//    {
//        foreach(var token in _outputTemplate.Tokens)
//        {
//            switch(token)
//            {
//                case TextToken textToken:
//                    RenderText(writer, textToken);
//                    break;

//                case PropertyToken propertyToken:
//                    RenderProperty(writer, propertyToken, context);
//                    break;

//                default:
//                    throw new NotSupportedException($"Unknown token type: {token.GetType().FullName}");
//            }
//        }
//    }

//    internal static void RenderText(TextWriter writer, TextToken token)
//    {
//        writer.Write(token.ToString());
//    }

//    private void RenderProperty(TextWriter writer, PropertyToken token, LoggerContext context)
//    {
//        if(token.Name == PropertyNames.Level)
//        {
//            var formatted = LevelFormatter.Format(context.Level, token.Format);

//            RenderHelper.Padding(writer, formatted, token.Alignment, token.Indent);
//        }
//        else if(token.Name == PropertyNames.NewLine)
//        {
//            RenderHelper.Padding(writer, Environment.NewLine, token.Alignment, token.Indent);
//        }
//        else if(token.Name == PropertyNames.Exception)
//        {
//            RenderHelper.Padding(writer, context.Exception?.ToString(), token.Alignment, token.Indent);
//        }
//        else if(token.Name == PropertyNames.MemberName)
//        {
//            RenderHelper.Padding(writer, context.MemberName, token.Alignment, token.Indent);
//        }
//        else if(token.Name == PropertyNames.FilePath)
//        {
//            RenderHelper.Padding(writer, context.FilePath, token.Alignment, token.Indent);
//        }
//        else if(token.Name == PropertyNames.LineNumber)
//        {
//            RenderHelper.Padding(writer, context.LineNumber?.ToString(), token.Alignment, token.Indent);
//        }
//        else
//        {
//            var sw = new StringWriter();

//            if(token.Name == PropertyNames.Message)
//            {
//                MessageTemplateRender.Render(
//                    context.MessageTemplate,
//                    context.Properties,
//                    sw,
//                    token.Format,
//                    _formatProvider);
//            }
//            else if(token.Name == PropertyNames.Timestamp)
//            {
//                DefaultRender(context.Timestamp, null, sw, token.Format, _formatProvider);
//            }
//            else if(token.Name == PropertyNames.Properties)
//            {
//                PropertiesFormatter.Format(
//                    context.MessageTemplate,
//                    context.Properties,
//                    _outputTemplate,
//                    sw,
//                    token.Format,
//                    _formatProvider);
//            }
//            else
//            {
//                // property
//                if(context.Properties.TryGetValue(token.Name, out var value))
//                {
//                    value.Render(sw, token.Format, _formatProvider);
//                }
//            }

//            RenderHelper.Padding(writer, sw.ToString(), token.Alignment, token.Indent);
//        }
//    }
//}
