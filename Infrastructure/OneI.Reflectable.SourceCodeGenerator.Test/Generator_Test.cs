namespace OneI.Reflectable;

using System.Threading.Tasks;
using OneT.CodeGenerator;
using VerifyXunit;

[UsesVerify]
public class Generator_Test : CodeGeneratorSnapshotTest
{
    [Fact]
    public Task source_code_generate()
    {
        var source = """
            #nullable enable
            namespace Test;

            using System;
            using OneI.Reflectable;

            [OneReflection]
            public class UserService
            {
                public void Register()
                {

                }
            }
            #nullable restore
            """;

        return Verify(source, new TypeCodeGenerator());
    }
}
