namespace OneI.Logable;

using OneI.Logable.Middlewares;

public class LoggerBuilder
{
    private readonly List<Func<LoggerDelegate, LoggerDelegate>> _componets;

    private readonly List<LoggerDelegate> _writers;

    public LoggerBuilder()
    {
        _componets = new();
        _writers = new();
    }

    #region Use

    public LoggerBuilder Use(Func<LoggerDelegate, LoggerDelegate> middleware)
    {
        _componets.Add(middleware);

        return this;
    }

    public LoggerBuilder Use(ILoggerMiddleware middleware)
    {
        _componets.Add(next =>
        {
            return context =>
            {
                middleware.Invoke(context, next);
            };
        });

        return this;
    }

    public LoggerBuilder Use<TMiddleware>()
        where TMiddleware : ILoggerMiddleware, new()
    {
        _componets.Add(next =>
        {
            return context =>
            {
                var middleware = new TMiddleware();

                middleware.Invoke(context, next);
            };
        });

        return this;
    }

    public LoggerBuilder Use(Action<LoggerContext, Action> middleware)
    {
        _componets.Add(next =>
        {
            return context =>
            {
                middleware.Invoke(context, _next);

                void _next() => next(context);
            };
        });

        return this;
    }

    public LoggerBuilder Use(Action<LoggerContext, LoggerDelegate> middleware)
    {
        _componets.Add(next =>
        {
            return context => middleware(context, next);
        });

        return this;
    }

    #endregion Use

    /// <summary>
    /// 开启一个新的管道（需要满足给定的<paramref name="condition"/>）
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public LoggerBuilder NewWhen(Func<LoggerContext, bool> condition, Action<LoggerBuilder> configuration)
    {
        Check.NotNull(condition);
        Check.NotNull(configuration);

        var builder = New();

        configuration(builder);

        Use(main =>
        {
            builder.End(main);

            var branch = builder.PackageComponents();

            return context =>
            {
                if(condition(context))
                {
                    branch(context);
                }

                main(context);
            };
        });

        return this;
    }

    /// <summary>
    /// 开启一个分支（需要满足给定的<paramref name="condition"/>）
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="branch"></param>
    /// <returns></returns>
    public LoggerBuilder MapWhen(Func<LoggerContext, bool> condition, LoggerDelegate branch)
    {
        Use(new MapMiddleware(branch, condition));

        return this;
    }

    #region Write To

    public LoggerBuilder WriteTo(Action<LoggerContext> writer)
    {
        _writers.Add(c => writer.Invoke(c));

        return this;
    }

    public LoggerBuilder WriteTo(ILoggerWriter writer)
    {
        WriteTo(c => writer.Write(c));

        return this;
    }

    #endregion Write To

    public Logger Build() => new(PackageComponents(), _writers);

    private void End(LoggerDelegate handler) => Use(_ => handler);

    private LoggerDelegate PackageComponents()
    {
        LoggerDelegate process = static context => { };

        for(var i = _componets.Count - 1; i >= 0; i--)
        {
            process = _componets[i](process);
        }

        return process;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static LoggerBuilder New() => new();
}
