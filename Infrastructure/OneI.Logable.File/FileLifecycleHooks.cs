namespace OneI.Logable;

using System;
using System.IO;
using System.Text;

/// <summary>
/// 允许挂接到日志文件生命周期事件。挂钩同步运行，因此如果执行长操作，可能会影响应用程序的响应。
/// </summary>
public abstract class FileLifecycleHooks
{
    /// <summary>
    /// 初始化或包装在日志文件上打开的<paramref name="underlyingStream"/>。
    /// 这可用于写入文件头，或将流包装在另一个添加缓冲、压缩、加密等功能的文件中。
    /// 调用此方法时，基础文件可能为空，也可能不为空。
    /// </summary>
    /// <remarks>必须从此方法的重写中返回值。Serilog将刷新和/或处理返回的值，但不会处理最初传入的流，除非它本身返回。</remarks>
    /// <param name="path"></param>
    /// <param name="underlyingStream"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public virtual Stream OnFileOpened(string path, Stream underlyingStream, Encoding encoding)
        => OnFileOpened(underlyingStream, encoding);

    public virtual Stream OnFileOpened(Stream underlyingStream, Encoding encoding) => underlyingStream;

    /// <summary>
    /// 在删除过时（滚动）日志文件之前调用。
    /// 这可用于将旧日志复制到存档位置或发送到备份服务器。
    /// </summary>
    /// <param name="path"></param>
    public virtual void OnFileDeleting(string path) { }

    /// <summary>
    /// 创建一个按顺序调用其方法的<see cref="FileLifecycleHooks"/>链。可用于组合<see cref="FileLifecycleHooks"/>。
    /// 例如：将头信息添加到每个日志文件中并对其进行压缩。
    /// </summary>
    /// <param name="next"></param>
    /// <returns></returns>
    public FileLifecycleHooks Then(FileLifecycleHooks next) => new FileLifeCycleHookChain(this, next);
}

/// <summary>
/// 钩子链条
/// </summary>
public class FileLifeCycleHookChain : FileLifecycleHooks
{
    private readonly FileLifecycleHooks _first;
    private readonly FileLifecycleHooks _second;

    public FileLifeCycleHookChain(FileLifecycleHooks first, FileLifecycleHooks second)
    {
        _first = first ?? throw new ArgumentNullException(nameof(first));
        _second = second ?? throw new ArgumentNullException(nameof(second));
    }

    public override Stream OnFileOpened(string path, Stream underlyingStream, Encoding encoding)
    {
        var firstStreamResult = _first.OnFileOpened(path, underlyingStream, encoding);
        var secondStreamResult = _second.OnFileOpened(path, firstStreamResult, encoding);

        return secondStreamResult;
    }

    public override void OnFileDeleting(string path)
    {
        _first.OnFileDeleting(path);
        _second.OnFileDeleting(path);
    }
}
