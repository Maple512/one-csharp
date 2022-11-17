namespace OneI.Logable;

using System.Collections.Generic;
using OneI.Logable.Properties;

public class LogContext : ILoggerContext
{
    public LogContext(ILogContent content, IEnumerable<ITextToken> tokens)
    {
        Content = content;
        Tokens = tokens;
    }

    public ILogContent Content { get; }

    public IEnumerable<IProperty> Properties { get; }

    public IEnumerable<ITextToken> Tokens { get; }

    public string? MemberName { get; init; }

    public string? FilePath { get; init; }

    public int? LineNumber { get; init; }
}
