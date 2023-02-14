namespace OneI.Logable;

public interface ILoggerPipelineConfiguration
{
    ILoggerConfiguration With(ILoggerMiddleware middleware);

    public ILoggerConfiguration With<TMiddleware>()
        where TMiddleware : ILoggerMiddleware, new()
        => With(new TMiddleware());

    public ILoggerConfiguration With(Action<LoggerMessageContext> middleware);

    public ILoggerConfiguration WithWhen(Func<LoggerMessageContext, bool> condition, ILoggerMiddleware middleware);

    public ILoggerConfiguration WithWhen<TMiddleware>(Func<LoggerMessageContext, bool> condition)
        where TMiddleware : ILoggerMiddleware, new()
        => WithWhen(condition, new TMiddleware());

    public ILoggerConfiguration WithWhen(Func<LoggerMessageContext, bool> condition, Action<LoggerMessageContext> middleware);
}
