namespace OneI.Logable;

internal readonly struct LoggerScope
{
    private readonly ILoggerMiddleware[] _middlewares;

    public LoggerScope(in ILoggerMiddleware[] middlewares)
    {
        _middlewares = middlewares;
    }

    public ILoggerMiddleware[] Middlewares
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _middlewares;
    }
}
