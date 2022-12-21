namespace OneI.Logable.Temporaries;

using System;
using OneI.Logable.Middlewares;
using OneI.Textable;
using OneI.Textable.Templating.Properties;

/// <summary>
/// 表示临时存放<see cref="ILoggerMiddleware"/>的容器
/// </summary>
public static class TemporaryContainer
{
    private static readonly AsyncLocal<List<ILoggerMiddleware>> _queue = new();

    public static IDisposable Push(params ILoggerMiddleware[] middlewares)
    {
        var queue = GetOrCreateQueue();

        var backup = new DisposeAction(() =>
        {
            _queue.Value = queue;
        });

        foreach(var middleware in middlewares)
        {
            queue.Add(middleware);
        }

        _queue.Value = queue;

        return backup;
    }

    public static IDisposable PushProperty<T>(string name, T value, IFormatter<T>? renderer = null)
    {
        return Push(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value, renderer)));
    }

    public static IDisposable PushProperty<T>(string name, T value)
        where T : IFormatter<T>
    {
        return Push(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value)));
    }

    /// <summary>
    /// 暂时清空容器中的所有<see cref="ILoggerMiddleware"/>，待释放后还原
    /// </summary>
    /// <returns></returns>
    public static IDisposable Suspend()
    {
        var queue = GetOrCreateQueue();

        var backup = new DisposeAction(() =>
        {
            _queue.Value = queue;
        });

        _queue.Value = new(0);

        return backup;
    }

    /// <summary>
    /// 清空容器中的所有<see cref="ILoggerMiddleware"/>
    /// </summary>
    public static void Clear()
    {
        _queue.Value = new(0);
    }

    internal static List<ILoggerMiddleware> GetOrCreateQueue()
    {
        var queue = _queue.Value;

        if(queue == null)
        {
            queue = new(0);

            _queue.Value = queue;
        }

        return queue;
    }
}
