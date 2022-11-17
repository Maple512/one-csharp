namespace OneI.Logable;

using System.IO;

public interface ILoggerTypeFormatter<T>
{
    bool IsSupported(string? format);

    void Format(T value, TextWriter output, string? format);
}
