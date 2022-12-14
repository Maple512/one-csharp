namespace OneI.Logable;

public class LoggerMiddleware_Test
{
    [Fact]
    public void order()
    {
        var order = new List<int>();

        var logger = new LoggerConfiguration()
            .Use(context => order.Add(1))
            .UseWhen(context => context.Level == LogLevel.Debug, _ => order.Add(2))
            .Use(context => order.Add(3))
            .CreateLogger();

        logger.Write(LogLevel.Information, "message");

        logger.Write(LogLevel.Debug, "message");

        order.Count.ShouldBe(5);

        order[0].ShouldBe(1);
        order[1].ShouldBe(3);
        order[2].ShouldBe(1);
        order[3].ShouldBe(2);
        order[4].ShouldBe(3);
    }
}
