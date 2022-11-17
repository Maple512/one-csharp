namespace OneI.Logable.TypeFormatting;

using System;
using System.IO;

public class StringFormatter : ILoggerTypeFormatter<string>
{
    private const char _upper_case = 'u',
                        _lower_case = 'l';

    public static readonly char[] _formats = new[]
    {
        'l','u'
    };

    public void Format(string message, TextWriter output, string? format)
    {
        throw new NotImplementedException();
    }

    public bool IsSupported(string? format)
    {
        throw new NotImplementedException();
    }
}
