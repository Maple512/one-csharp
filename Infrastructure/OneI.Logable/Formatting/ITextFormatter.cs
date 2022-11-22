namespace OneI.Logable.Formatting;

using System.IO;

public interface ITextFormatter
{
    void Format(LoggerContext context, TextWriter output);
}
