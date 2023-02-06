namespace OneI.Logable;

using OneI.Logable.Fakes;
using OneI.Logable.Middlewares;
using OneI.Logable.Templates;

public class Logger_Test
{
    [Fact]
    public void level_enalble_state()
    {
        // default level
        var logger = new LoggerConfiguration().CreateLogger();

        logger.IsEnable(LogLevel.Verbose).ShouldBeTrue();
        logger.IsEnable(LogLevel.Fatal).ShouldBeTrue();

        // override type
        logger = new LoggerConfiguration()
                 .Level.Override<Logger_Test>(LogLevel.Information)
                 .CreateLogger();

        logger.ForContext<Logger_Test>().IsEnable(LogLevel.Verbose).ShouldBeFalse();
        logger.ForContext<Logger_Test>().IsEnable(LogLevel.Information).ShouldBeTrue();
        logger.ForContext<Logger_Test>().IsEnable(LogLevel.Fatal).ShouldBeTrue();

        // other type is true (still the default)
        logger.ForContext<ILogger>().IsEnable(LogLevel.Verbose).ShouldBeTrue();

        logger.Information("", 1);
    }

    [Fact]
    public void begin_scope()
    {
        var order = new List<int>(100);

        var id = Guid.NewGuid();
        var name = "Id";
        object propertyId = null!;

        var logger = Fake.CreateLogger(
                                       logger: logger => logger
                                                         .Use(new ActionMiddleware(_ => order.Add(1)))
                                                         .Use(new ActionMiddleware(_ => order.Add(2)))
                                                         .Audit.Attach(c =>
                                                         {
                                                             c.Properties.TryGetValue(name, out propertyId);
                                                         }));

        var scope = new ILoggerMiddleware[]
        {
            new ActionMiddleware(_ => order.Add(3)), new PropertyMiddleware<Guid>(name, id)
            , new ActionMiddleware(_ => order.Add(4)),
        };

        using(logger.BeginScope("1", 1))
        {
            logger.Error($"{{{name}}}");

            order[0].ShouldBe(1);
            order[1].ShouldBe(2);
            order[2].ShouldBe(3);
            order[3].ShouldBe(4);

            propertyId.ShouldNotBeNull();
            propertyId.ToString().ShouldBe(id.ToString());
        }

        logger.Error("{Id}");

        propertyId.ShouldBeNull();

        order[4].ShouldBe(1);
        order[5].ShouldBe(2);
    }

    [Fact]
    public void logger_test()
    {
        var logger = Fake.CreateLogger(logger: configure =>
        {
            configure.Template.UseWhen(c => c.Exception is not null, Fake.ErrorTemplate);
        });

        logger.ForContext<Middleware_Test>().Error(new ArgumentException(), "Include Source Context");

        logger.Error("Exclude Source Context");

        logger.ForContext<IServiceProvider>().Error(new ArgumentException(), "Include Source Context");

        logger.Error("Exclude Source Context");

        logger.Error(
                     "{0} {1} {2} {3}{NewLine}{FileName'4}#L{LineNumber}@{MemberName}",
                     "0",
                     1,
                     new Dictionary<int, int> { { 2, 3 }, },
                     new List<int> { 4, 5, 6, });
    }
}
