namespace OneI.Logable.Configurations;

using System;

public interface ILoggerEndpointBuilder
{
    ILoggerBuilder Run(ILoggerEndpoint writer);

    ILoggerBuilder RunWhen(Func<LoggerContext, bool> condition, ILoggerEndpoint writer);
}
