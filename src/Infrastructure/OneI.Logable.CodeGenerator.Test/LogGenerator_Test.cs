namespace OneI.Logable;
using System.Threading.Tasks;
using OneI.Logable.CodeGenerated;
using OneT.CodeGenerator;
using VerifyXunit;

[UsesVerify]
public class LogGenerator_Test : CodeGeneratorSnapshotTest
{
    [Fact]
    public Task code_generated()
    {
        var source =
"""
#nullable enable
namespace OneI.Logable.Fakes;

using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using OneI.Logable;
using OneI.Logable.CodeGenerated;

public class UserService
{
    public void Register()
    {
        var logger = new LoggerConfiguration()
        .CreateLogger();

        logger.Debug("",1,new List<int>(0));
    }
}
#nullable restore
""";

        //var logger = (ILogger)default!;

        //logger.Write(LogLevel.Debug, $"{nameof(Guid)}", 1, 2, 3);

        return Verify(source, new LoggerCodeGenerator());
    }
}
