namespace OneI.Logable;

internal class LoggerScope
{
    public LoggerScope(ILoggerMiddleware[] middlewares)
    {
        Middlewares = middlewares;
    }

    public ILoggerMiddleware[] Middlewares
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
}
