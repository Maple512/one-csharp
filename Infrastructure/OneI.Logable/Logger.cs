namespace OneI.Logable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class Logger : ILogger
{
    private readonly IEnumerable<LogDelegate> _branchs;
    private readonly IEnumerable<Func<ILoggerContext, Task<bool>>> _filters;

    public Logger(
        IEnumerable<Func<ILoggerContext, Task<bool>>> filters,
        IEnumerable<LogDelegate> branchs)
    {
        _branchs = branchs;
        _filters = filters;
    }

    public async Task WriteAsync(
        ILogContent content,
        [CallerMemberName] string? membername = null,
        [CallerFilePath] string? filepath = null,
        [CallerLineNumber] int? line = null)
    {


        var context = new LogContext(content, Enumerable.Empty<ITextToken>())
        {
            FilePath = filepath,
            LineNumber = line,
            MemberName = membername,
        };

        var passed = true;
        foreach(var item in _filters)
        {
            passed = await item(context);

            if(passed == false)
            {
                break;
            }
        }

        if(passed)
        {
            foreach(var item in _branchs)
            {
                await item(context);
            }
        }
    }

    public async Task WriteAsync<T>(
        LogLevel level,
        string message,
        T parameter,
        [CallerMemberName] string? membername = null,
        [CallerFilePath] string? filepath = null,
        [CallerLineNumber] int? line = null)
    {
        await WriteAsync(new LogContent(level, message, parameters: new object?[] { parameter }), membername, filepath, line);
    }
}
