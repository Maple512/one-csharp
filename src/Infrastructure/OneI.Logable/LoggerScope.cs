namespace OneI.Logable;

internal readonly struct LoggerScope
{
    public LoggerScope(in ILoggerMiddleware[] middlewares) => Middlewares = middlewares;

    public ILoggerMiddleware[] Middlewares
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
}
