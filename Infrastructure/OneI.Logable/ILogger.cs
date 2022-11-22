namespace OneI.Logable;

using System;
using System.Runtime.CompilerServices;

public interface ILogger
{
    bool IsEnabled(LogLevel level);

    void Write(LogLevel level, string message, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int? lineNumber = null)
        => Write(level, message, parameter: null, memberName, filePath, lineNumber);

    void Write(LogLevel level, string message, Exception exception, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int? lineNumber = null)
        => Write(level, message, null, exception, memberName, filePath, lineNumber);

    void Write(LogLevel level, string message, object?[]? parameter, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int? lineNumber = null)
        => Write(level, message, parameter, null, memberName, filePath, lineNumber);

    void Write(LogLevel level, string message, object?[]? parameter, Exception? exception = null, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int? lineNumber = null);

    void Write(in LoggerContext context);
}
