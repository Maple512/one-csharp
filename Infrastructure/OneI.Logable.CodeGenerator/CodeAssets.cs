namespace OneI.Logable;

internal static class CodeAssets
{
    public const string LogableNamespace = $"OneI.Logable";

    public const string LogClassFullName = $"{LogableNamespace}.Log";
    public const string LogExtensionsFileName = "Log.extension.g.cs";
    public const string LogFileName = "Log.g.cs";
    public const string LogFileContent = """
        // <auto-generated/>
        #nullable enable
        namespace OneI.Logable;

        using System;
        using System.Collections.Generic;
        using System.Linq;
        using OneI.Logable.Infrastructure;
        using OneI.Textable.Templating.Properties;

        public static partial class Log
        {
            private static ILogger _logger = NullLogger.Instance;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="logger"></param>
            public static void Initialize(ILogger logger)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public static bool IsEnable(LogLevel logLevel)
            {
                return _logger.IsEnable(logLevel);
            }

            #region Write

            public static void Write(LogLevel level, string message)
            {
                LoggerExtensions.PackageWrite(_logger, level, null, message, null);
            }

            public static void Write(LogLevel level, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, level, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Write(LogLevel level, Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(_logger, level, exception, message, null);
            }

            public static void Write(LogLevel level, Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, level, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Write(LoggerContext context)
            {
                _logger?.Write(context);
            }

            #endregion Write

            #region Verbose

            public static void Verbose(string message)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Verbose, null, message);
            }

            public static void Verbose(string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Verbose, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Verbose(Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Verbose, exception, message, null);
            }

            public static void Verbose(Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Verbose, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Verbose

            #region Debug

            public static void Debug(string message)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Debug, null, message);
            }

            public static void Debug(string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Debug, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Debug(Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Debug, exception, message, null);
            }

            public static void Debug(Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Debug, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Debug

            #region Information

            public static void Information(string message)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Information, null, message);
            }

            public static void Information(string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Information, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Information(Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Information, exception, message, null);
            }

            public static void Information(Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Information, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Information

            #region Warning

            public static void Warning(string message)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Warning, null, message);
            }

            public static void Warning(string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Warning, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Warning(Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Warning, exception, message, null);
            }

            public static void Warning(Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Warning, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Warning

            #region Error

            public static void Error(string message)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Error, null, message);
            }

            public static void Error(string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Error, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Error(Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Error, exception, message, null);
            }

            public static void Error(Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Error, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Error

            #region Fatal

            public static void Fatal(string message)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Fatal, null, message);
            }

            public static void Fatal(string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Fatal, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Fatal(Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Fatal, exception, message, null);
            }

            public static void Fatal(Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(_logger, LogLevel.Fatal, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Fatal
        }
        #nullable restore
        """;

    public const string LoggerPropertyCreatorClassName = "LoggerPropertyCreator";
    public const string LoggerPropertyCreatorClassFileName = $"{LoggerPropertyCreatorClassName}.g.cs";
    public const string LoggerPropertyCreateMethodName = "Create";
    public const string LoggerPropertyCreateCalledName = $"{LoggerPropertyCreatorClassName}.{LoggerPropertyCreateMethodName}";

    public const string MessageParameterName = "message";
    public const string MessageParameterType = "global::System.String";

    public const string LogLevelParameterName = "level";
    public const string LogLevelParameterType = "global::OneI.Logable.LogLevel";

    public const string ExceptionParameterName = "exception";
    public const string ExceptionParameterType = "global::System.HasException";

    public const string LoggerFullName = $"{LogableNamespace}.ILogger";
    public const string LoggerExtensionFullName = "OneI.Logable.LoggerWriteExtensions";
    public const string LoggerExtensionClassName = "LoggerWriteExtensions";
    public const string LoggerExtensionClassFileName = $"{LoggerExtensionClassName}.g.cs";
    public const string LoggerExtensionExtensionClassFileName = $"{LoggerExtensionClassName}.extension.g.cs";

    public const string LoggerExtensionClassContent = """
        // <auto-generated/>
        #nullable enable
        namespace OneI.Logable;

        using System;
        using System.Collections.Generic;
        using System.Linq;
        using OneI.Logable.Infrastructure;
        using OneI.Textable.Templating.Properties;

        public static partial class LoggerWriteExtensions
        {
            #region Write

            public static void Write(this ILogger logger, LogLevel level, string message)
            {
                LoggerExtensions.PackageWrite(logger, level, null, message, null);
            }

            public static void Write(this ILogger logger, LogLevel level, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, level, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Write(this ILogger logger, LogLevel level, Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(logger, level, exception, message, null);
            }

            public static void Write(this ILogger logger, LogLevel level, Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, level, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Write

            #region Verbose

            public static void Verbose(this ILogger logger, string message)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Verbose, null, message);
            }

            public static void Verbose(this ILogger logger, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Verbose, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Verbose(this ILogger logger, Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Verbose, exception, message, null);
            }

            public static void Verbose(this ILogger logger, Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Verbose, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Verbose

            #region Debug

            public static void Debug(this ILogger logger, string message)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Debug, null, message);
            }

            public static void Debug(this ILogger logger, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Debug, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Debug(this ILogger logger, Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Debug, exception, message, null);
            }

            public static void Debug(this ILogger logger, Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Debug, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Debug

            #region Information

            public static void Information(this ILogger logger, string message)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Information, null, message);
            }

            public static void Information(this ILogger logger, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Information, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Information(this ILogger logger, Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Information, exception, message, null);
            }

            public static void Information(this ILogger logger, Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Information, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Information

            #region Warning

            public static void Warning(this ILogger logger, string message)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Warning, null, message);
            }

            public static void Warning(this ILogger logger, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Warning, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Warning(this ILogger logger, Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Warning, exception, message, null);
            }

            public static void Warning(this ILogger logger, Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Warning, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Warning

            #region Error

            public static void Error(this ILogger logger, string message)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Error, null, message);
            }

            public static void Error(this ILogger logger, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Error, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Error(this ILogger logger, Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Error, exception, message, null);
            }

            public static void Error(this ILogger logger, Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Error, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Error

            #region Fatal

            public static void Fatal(this ILogger logger, string message)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Fatal, null, message);
            }

            public static void Fatal(this ILogger logger, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Fatal, null, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            public static void Fatal(this ILogger logger, Exception exception, string message)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Fatal, exception, message, null);
            }

            public static void Fatal(this ILogger logger, Exception exception, string message, params object?[] args)
            {
                LoggerExtensions.PackageWrite(logger, LogLevel.Fatal, exception, message, args.Select(x => PropertyValue.CreateLiteral(x)));
            }

            #endregion Fatal
        }
        #nullable restore
        """;
}
