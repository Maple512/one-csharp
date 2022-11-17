namespace OneI.Logable;

using System.IO;

public interface ILoggerFormatter
{
    void Format(ILoggerContext context, TextWriter output);
}
