namespace OneI.Logable.Internal;

internal static class CodeAssets
{
    public const string Logable = "Logable";
    public const string LogableNamespace = $"OneI.{Logable}";

    public const string LoggerFullName = $"{LogableNamespace}.ILogger";

    public const string MessageParameterName = "message";
    public const string MessageParameterType = "global::System.String";
    public const string LogLevelParameterName = "level";
    public const string LogLevelParameterType = $"global::{LogableNamespace}.LogLevel";
    public const string ExceptionParameterName = "exception";
    public const string ExceptionParameterType = "global::System.Exception";

    public static class PropertyFactory
    {
        public const string FileName = "OneILoggerExtensions.f.g.cs";
    }

    public static class LoggerExtension
    {
        public const string Name = "OneILoggerCodeGeneratorExtensions";
        public const string ClassFullName = $"{LogableNamespace}.{Name}";
        public const string FileNmae = "OneILoggerExtensions.g.cs";
        public const string PartialName = "OneILoggerExtensions.p.g.cs";
        public const string Content =
"""
#nullable enable
namespace OneI.Logable;

using global::System;
using global::System.ComponentModel;
using global::System.Diagnostics;
using global::OneI.Logable.Middlewares;
using global::OneI.Logable.Templates;

[global::System.Diagnostics.DebuggerStepThrough]
internal static partial class OneILoggerCodeGeneratorExtensions
{
    #region Write

    [Conditional("DEBUG")]
    public static void Write(this ILogger logger, LogLevel level, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Write(this ILogger logger, LogLevel level, Exception exception, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Write(this ILogger logger, LogLevel level, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

    #endregion Write

    #region Verbose

    [Conditional("DEBUG")]
    public static void Verbose(this ILogger logger, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Verbose(this ILogger logger, Exception exception, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Verbose(this ILogger logger, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

    #endregion Verbose

    #region Debug

    [Conditional("DEBUG")]
    public static void Debug(this ILogger logger, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Debug(this ILogger logger, Exception exception, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Debug(this ILogger logger, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

    #endregion Debug

    #region Information

    [Conditional("DEBUG")]
    public static void Information(this ILogger logger, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Information(this ILogger logger, Exception exception, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Information(this ILogger logger, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

    #endregion Information

    #region Warning

    [Conditional("DEBUG")]
    public static void Warning(this ILogger logger, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Warning(this ILogger logger, Exception exception, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Warning(this ILogger logger, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

    #endregion Warning

    #region Error

    [Conditional("DEBUG")]
    public static void Error(this ILogger logger, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Error(this ILogger logger, Exception exception, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Error(this ILogger logger, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

    #endregion Error

    #region Fatal

    [Conditional("DEBUG")]
    public static void Fatal(this ILogger logger, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Fatal(this ILogger logger, Exception exception, params object?[] args)
        => throw new NotSupportedException();

    [Conditional("DEBUG")]
    public static void Fatal(this ILogger logger, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

    #endregion Fatal
}
#nullable restore
""";
    }
}
