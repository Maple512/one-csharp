namespace OneI.Logable;

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public interface ILogger
{
    Task WriteAsync<T>(
        LogLevel level,
        string message,
        T parameter,
        [CallerMemberName] string? membername = null,
        [CallerFilePath] string? filepath = null,
        [CallerLineNumber] int? line = null);

    Task WriteAsync(
        ILogContent content,
        [CallerMemberName] string? membername = null,
        [CallerFilePath] string? filepath = null,
        [CallerLineNumber] int? line = null);
}
