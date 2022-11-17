namespace OneI.Logable;

using System.Collections.Generic;
using OneI.Logable.Properties;

public interface ILoggerContext
{
    string? MemberName { get; }

    string? FilePath { get; }

    int? LineNumber { get; }

    ILogContent Content { get; }

    IEnumerable<ITextToken> Tokens { get; }

    IEnumerable<IProperty> Properties { get; }
}
