namespace OneI.Logable;

using OneT.CodeGenerator;

[UsesVerify]
public class LogGenerator_Test : CodeGeneratorSnapshotTest
{
    [Fact]
    public Task generator_simple()
    {
        var source = """
            #nullable enable
            namespace Test;

            using System;
            using System.Collections;
            using System.Threading.Tasks;
            using System.Collections.Generic;
            using OneI.Logable;

            public class UserService
            {
                private ILogger logger;

                public async Task Register()
                {
                    Log.Error("", "", new object(), new Dictionary<int, int>(), new List<int>());

                    //var logger = new LoggerConfiguration()
                    //.CreateLogger();

                    logger.Error("", "", new object(), new Dictionary<int, int>(), new List<int>());

                        logger.IsEnable(LogLevel.Verbose);
                    logger.IsEnable(LogLevel.Fatal);

                    // override type
                    logger = new LoggerConfiguration()
                       .Level.Override<UserService>(LogLevel.Information)
                       .CreateLogger() ;

                    logger.ForContext<UserService>().IsEnable(LogLevel.Verbose);

                    logger.Information("", 1, 2, 3, 4, 5);
                    logger.Information("", 1, 2, 3, 4, 5);
                    logger.Information("", 1, 2, 3, 4, 5);
            

                        using(logger.BeginScope())
                        {
                            logger.Error("name");
                        }

                            logger.ForContext<UserService>().Error(new ArgumentException(), "Include Source Context");

            logger.Error("Exclude Source Context");

            logger.ForContext<UserService>().Error(new ArgumentException(), "Include Source Context");

            logger.Error("Exclude Source Context");

            logger.Error(
                "{0} {1} {2} {3}{NewLine}{FileName'4}#L{LineNumber}@{MemberName}",
                "0",
                1,
                new Dictionary<int, int> { { 2, 3 } },
                new List<int> { 4, 5, 6 });
                }

            [Serializable]
            public class User
            {
                public int Id { get; set; }
            }
            }

            public struct SturctModel1
            {
                public int Id { get; set; }
            }
            #nullable restore
            """;

        return Verify(source, new LoggerCodeGenerator());
    }

    [Fact]
    public async void object_parameter()
    {
        var a1 = await Task.FromResult(1);

        var bb = (dynamic)1;
        var a  = new SturctModel1 { Id = 1, };

        var b = a with { Id = 2, };
    }

    public struct SturctModel1
    {
        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public int Id { get; set; }
    }
}
