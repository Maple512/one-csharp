namespace OneI.Logable;

using System;
using System.IO;

public static class InternalLog
{
    private static Action<string>? _log;

    public static void Enable(Action<string> log)
    {
        _log = log;
    }

    public static void Enable(TextWriter output)
    {
        Enable(x =>
        {
            output.WriteLine(x);
            output.Flush();
        });
    }

    public static void Disable()
    {
        _log = null;
    }

    public static void WriteLine(string format, params object[] args)
    {
        _log?.Invoke(string.Format($"{DateTimeOffset.Now:O} {format}", args));
    }
}
