namespace OneI.Logable;

public interface ILoggerConfiguration : ILoggerPipelineConfiguration
{
    ILoggerTemplateConfiguration Template { get; }

    ILoggerLevelConfiguration Level { get; }

    ILoggerPropertyConfiguration Properties { get; }

    ILoggerSinkConfiguration Sink { get; }

    ILoggerAuditConfiguration Audit { get; }

    ILogger CreateLogger();

    public ILogger<TSource> CreateLogger<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TSource>()
        => new Logger<TSource>(CreateLogger());
}
