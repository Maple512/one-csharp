namespace OneI.Logable;

using System.Threading.Tasks;
using OneT.CodeGenerator;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class LogGenerator_Test : CodeGeneratorSnapshotTest
{
    [Fact]
    public Task generator_simple()
    {
        // The source code to test
        var source = """
            #nullable enable
            namespace Test;

            using System;
            using System.Collections;
            using System.Collections.Generic;
            using OneI.Logable;

            public class UserService
            {
                public void Register()
                {
                    var logger = new LoggerConfiguration()
                        .CreateLogger();

                    var user = new User(){Id =1};

                    logger.Debug("", user);

                    Log.Debug("", user);
                }
            }

            [Serializable]
            public class User
            {
                public int Id { get; set; }
            }
            #nullable restore
            """;

        return Verify(source, new LoggerCodeGenerator());
    }
}
