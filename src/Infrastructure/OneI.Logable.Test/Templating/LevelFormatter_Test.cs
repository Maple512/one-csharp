namespace OneI.Logable.Templating;

using OneI.Logable;
using OneI.Logable.Fakes;
using OneI.Logable.Formatters;

public class LevelFormatter_Test
{
    [Theory]
    [InlineData(LogLevel.Verbose, "l1", "v")]
    [InlineData(LogLevel.Verbose, "l2", "vrb")]
    [InlineData(LogLevel.Verbose, "l3", "verbose")]
    [InlineData(LogLevel.Verbose, "3l", "verbose")]
    [InlineData(LogLevel.Information, "u1", "I")]
    [InlineData(LogLevel.Information, "u2", "INF")]
    [InlineData(LogLevel.Information, "u3", "INFORMATION")]
    [InlineData(LogLevel.Information, "3u", "INFORMATION")]
    [InlineData(LogLevel.Fatal, "1", "F")]
    [InlineData(LogLevel.Fatal, "2", "Ftl")]
    [InlineData(LogLevel.Fatal, "3", "Fatal")]
    public void level_format(LogLevel level, string? format, string expected)
    {
        var formatter = new LevelFormatter();

        formatter.ToDisplayString(level, format)
            .ShouldBe(expected);
    }

    [Theory]
    [InlineData(LogLevel.Fatal, "0", "Fatal")]
    [InlineData(LogLevel.Fatal, "2", "Ftl")]
    [InlineData(LogLevel.Fatal, "3", "Fatal")]
    [InlineData(LogLevel.Fatal, "5", "Fatal")]
    public void invalid_format(LogLevel level, string? format, string expected)
    {
        var formatter = new LevelFormatter();

        formatter.ToDisplayString(level, format).ShouldBe(expected);
    }
}
