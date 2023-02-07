namespace OneI.Logable;
using System.Threading.Tasks;
using OneT.CodeGenerator;
using VerifyXunit;

[UsesVerify]
public class LogGenerator_Test : CodeGeneratorSnapshotTest
{
    [Fact]
    public Task logger_extensions_code_generated()
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

public class UserService
{
    public void Register()
    {
        var logger = new LoggerConfiguration()
        .CreateLogger();

        logger.Error("",1);
    }
}
#nullable restore
""";

        return Verify(source, new LoggerCodeGenerator());
    }
}
