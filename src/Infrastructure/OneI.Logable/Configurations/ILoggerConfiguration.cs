namespace OneI.Logable;

using OneI.Logable.Configurations;

public interface ILoggerConfiguration : ILoggerPipelineConfiguration
{
    /// <summary>
    ///     表示日志模板相关配置
    /// </summary>
    ILoggerTemplateConfiguration Template { get; }

    /// <summary>
    ///     表示<see cref="ILoggerLevelConfiguration" />相关的配置
    /// </summary>
    ILoggerLevelConfiguration Level { get; }

    /// <summary>
    ///     表示<see cref="ILoggerPropertyConfiguration" />相关的配置
    /// </summary>
    ILoggerPropertyConfiguration Properties { get; }

    /// <summary>
    ///     表示<see cref="ILoggerSink" />相关的配置
    /// </summary>
    ILoggerSinkConfiguration Sink { get; }

    ILoggerAuditConfiguration Audit { get; }

    /// <summary>
    ///     创建一个<see cref="ILogger" />
    /// </summary>
    /// <returns></returns>
    ILogger CreateLogger();

    ILogger<T> CreateLogger<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>();
}
