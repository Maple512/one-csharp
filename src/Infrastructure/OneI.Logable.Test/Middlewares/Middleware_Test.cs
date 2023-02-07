namespace OneI.Logable.Middlewares;

public class Middleware_Test
{
    [Theory]
    [InlineData(LogLevel.Verbose, 2)]
    [InlineData(LogLevel.Information, 3)]
    [InlineData(LogLevel.Debug, 2)]
    public void middleware_order(LogLevel level, int count)
    {
        var orders = new List<int>();
        var logger = new LoggerConfiguration()
                     .With(_ => orders.Add(1))
                     .WithWhen(c => c.Level == level, _ => orders.Add(2))
                     .With(_ => orders.Add(3))
                     .CreateLogger();

        logger.Information("");

        orders.Count.ShouldBe(count);
        orders.First().ShouldBe(1);
        orders.Last().ShouldBe(3);
    }
}
