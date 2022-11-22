namespace OneI.Logable.Configurations;

using System;
using System.Collections.Generic;
using OneI.Logable;
using OneI.Logable.Enrichers;

public class LoggerEnrichmentConfiguration
{
    private readonly LoggerConfiguration _loggerConfiguration;
    private readonly Action<ILoggerEnricher> _enricherAction;

    public LoggerEnrichmentConfiguration(LoggerConfiguration logger, Action<ILoggerEnricher> enricherAction)
    {
        _loggerConfiguration = logger;
        _enricherAction = enricherAction;
    }

    public LoggerConfiguration With(params ILoggerEnricher[] enrichers)
    {
        foreach(var enricher in enrichers)
        {
            _enricherAction(enricher);
        }

        return _loggerConfiguration;
    }

    public LoggerConfiguration With<TEnricher>()
        where TEnricher : ILoggerEnricher, new()
    {
        return With(new TEnricher());
    }

    public LoggerConfiguration WithProperty(string name, object value, bool deconstruct = false)
    {
        return With(new PropertyEnricher(name, value, deconstruct));
    }

    public LoggerConfiguration FromContainer() => With<ContainerEnricher>();

    public LoggerConfiguration When(Func<LoggerContext, bool> condition, Action<LoggerEnrichmentConfiguration> configurationAction)
    {
        return Wrap(
            this,
            e => new ConditionalEnricher(e, condition),
            configurationAction);
    }

    public LoggerConfiguration AtLevel(LogLevel level, Action<LoggerEnrichmentConfiguration> configurationAction)
    {
        return Wrap(
            this,
            e => new ConditionalEnricher(e, context => context.Level >= level),
            configurationAction);
    }

    public LoggerConfiguration AtLevel(LogLevelSwitch levelSwitch, Action<LoggerEnrichmentConfiguration> configurationAction)
    {
        return Wrap(
            this,
            e => new ConditionalEnricher(e, context => context.Level >= levelSwitch.MinimumLevel),
            configurationAction);
    }

    /// <summary>
    /// 包装Sinks
    /// </summary>
    /// <param name="enrichmentConfiguration"></param>
    /// <param name="wraper"></param>
    /// <param name="enrichmentConfigurationAction"></param>
    /// <returns></returns>
    public static LoggerConfiguration Wrap(
        LoggerEnrichmentConfiguration enrichmentConfiguration,
        Func<ILoggerEnricher, ILoggerEnricher> wraper,
        Action<LoggerEnrichmentConfiguration> enrichmentConfigurationAction)
    {
        var wraps = new List<ILoggerEnricher>();

        var capturingConfiguration = new LoggerConfiguration();

        var capturingEnrichmentConfiguration = new LoggerEnrichmentConfiguration(
            capturingConfiguration,
            wraps.Add);

        capturingConfiguration.Enrich = capturingEnrichmentConfiguration;

        enrichmentConfigurationAction(capturingEnrichmentConfiguration);

        if(wraps.Count == 0)
        {
            return enrichmentConfiguration._loggerConfiguration;
        }

        var enclosed = wraps.Count == 1
            ? wraps[0]
            : new SafeAggregateEnricher(wraps);

        var wrappedEnricher = wraper(enclosed);

        if(wrappedEnricher is not IDisposable)
        {
            // log
            /*
             SelfLog.WriteLine("Wrapping enricher {0} does not implement IDisposable; to ensure " +
                              "wrapped enrichers are properly disposed, wrappers should dispose " +
                              "their wrapped contents", wrappedEnricher);
             */
        }

        return enrichmentConfiguration.With(wrappedEnricher);
    }
}
