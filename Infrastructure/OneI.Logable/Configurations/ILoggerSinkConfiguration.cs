namespace OneI.Logable.Configurations;

using System;

public interface ILoggerSinkConfiguration
{
    ILoggerConfiguration Use(ILoggerSink writer);

    ILoggerConfiguration UseWhen(Func<LoggerContext, bool> condition, ILoggerSink writer);
}
