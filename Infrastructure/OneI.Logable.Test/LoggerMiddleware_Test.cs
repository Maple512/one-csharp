namespace OneI.Logable;
public class LoggerMiddleware_Test
{
    [Fact]
    public void order()
    {
        var order = new List<int>();

        var logger = new LoggerBuilder()
            .Use((_, next) =>
            {
                order.Add(1);

                next();
            })
            .Use((_, next) =>
            {
                order.Add(2);

                next();
            })
            .Use((_, next) =>
            {
                order.Add(3);

                next();
            })
            .Build();

        logger.Write(LogLevel.Debug, "message");

        order.Count.ShouldBe(3);

        order[0].ShouldBe(1);
        order[1].ShouldBe(2);
        order[2].ShouldBe(3);
    }

    [Fact]
    public void map_when()
    {
        var order = new List<int>();

        var builder = new LoggerBuilder()
            .MapWhen(context => context.Level == LogLevel.Error,
            _ => order.Add((int)LogLevel.Error));

        builder.MapWhen(context => context.Level is LogLevel.Warning or LogLevel.Error,
            _ => order.Add((int)LogLevel.Warning));

        var logger = builder.Build();

        logger.Write(LogLevel.Error, string.Empty);

        order[0].ShouldBe(4);
        order[1].ShouldBe(3);

        logger.Write(LogLevel.Warning, string.Empty);

        order[2].ShouldBe(3);

        order.Count.ShouldBe(3);
    }
}
