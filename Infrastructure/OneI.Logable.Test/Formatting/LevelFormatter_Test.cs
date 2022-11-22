namespace OneI.Logable.Formatting;

public class LevelFormatter_Test
{
    [Theory]
    [InlineData((LogLevel)14, "t11", "14")]
    [InlineData(LogLevel.Information, "t11", "Information")]
    [InlineData(LogLevel.Debug, "t11", "Debug")]
    [InlineData(LogLevel.Debug, "w3", "dbg")]
    [InlineData(LogLevel.Debug, "s3", "Debug")]
    public void level_moniker(LogLevel level, string? format, string expected)
    {
        var result = LevelFormatter.GetLevelMoniker(level, format);

        result.ShouldBe(expected);
    }
}
