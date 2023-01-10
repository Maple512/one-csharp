namespace OneI.Logable;

using OneI.Logable.Fakes;
using OneI.Logable.Middlewares;
using OneI.Textable.Templating.Properties;

public class Logger_Test
{
    [Fact]
    public void logger_test()
    {
        var logger = Fake.CreateLogger(
            _ => { },
            options => options.RenderWhen(c => c.Exception is not null, Fake.ErrorTemplate));

        logger.ForContext<Middleware_Test>().Error(new ArgumentException(), "Include Source Context");

        logger.Error("Exclude Source Context");

        logger.ForContext<IServiceProvider>().Error(new ArgumentException(), "Include Source Context");

        logger.Error("Exclude Source Context");

        logger.Error("{0} {1} {2} {3}{NewLine}{FileName'4}#L{LineNumber}@{MemberName}", "0", 1, new Dictionary<int, int> { { 2, 3 } }, new List<int> { 4, 5, 6 });
    }

    [Fact]
    public void begin_scope()
    {
        var order = new List<int>(100);

        var id = Guid.NewGuid();
        var name = "Id";
        var propertyId = (PropertyValue?)null;

        var logger = Fake.CreateLogger(
            logger: logger => logger
            .Use(new ActionMiddleware(() => order.Add(1)))
            .Use(new ActionMiddleware(() => order.Add(2)))
            .Audit.Attach(c => c.Properties.TryGetValue(name, out propertyId)),
            file: static options => { });

        var scope = new ILoggerMiddleware[]
        {
            new ActionMiddleware(() => order.Add(3)),
            new PropertyMiddleware(name, PropertyValue.CreateLiteral(id)),
            new ActionMiddleware(() => order.Add(4)),
        };

        using(logger.BeginScope(scope))
        {
            logger.Error($"{{{name}}}");

            order[0].ShouldBe(3);
            order[1].ShouldBe(4);
            order[2].ShouldBe(1);
            order[3].ShouldBe(2);

            propertyId.ShouldNotBeNull();
            propertyId.ToString().ShouldBe(id.ToString());
        }

        logger.Error("{Id}");

        propertyId.ShouldBeNull();

        order[4].ShouldBe(1);
        order[5].ShouldBe(2);
    }

    private class ActionMiddleware : ILoggerMiddleware
    {
        private readonly Action _action;

        public ActionMiddleware(Action action)
        {
            _action = action;
        }

        public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
        {
            _action();

            return next(context);
        }
    }
}
