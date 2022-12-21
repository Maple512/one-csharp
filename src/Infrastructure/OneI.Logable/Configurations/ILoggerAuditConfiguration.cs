namespace OneI.Logable.Configurations;

using System;
using OneI.Logable.Middlewares;

public interface ILoggerAuditConfiguration
{
    /// <summary>
    /// 在管道中附加一个审计器
    /// </summary>
    /// <param name="audit"></param>
    /// <returns></returns>
    ILoggerConfiguration Attach(AuditDelegate audit);

    /// <summary>
    /// 在管道中附加一个审计器
    /// </summary>
    /// <param name="audit"></param>
    /// <returns></returns>
    ILoggerConfiguration Attach(Action<LoggerContext> audit);

    /// <summary>
    /// 在管道中附加一个审计器
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="audit"></param>
    /// <returns></returns>
    ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, AuditDelegate audit);

    /// <summary>
    /// 在管道中附加一个审计器
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="audit"></param>
    /// <returns></returns>
    ILoggerConfiguration AttachWhen(Func<LoggerContext, bool> condition, Action<LoggerContext> audit);
}
