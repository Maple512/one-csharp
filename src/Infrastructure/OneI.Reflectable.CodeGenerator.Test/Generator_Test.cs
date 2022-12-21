namespace OneI.Reflectable;

using System.Threading.Tasks;
using OneT.CodeGenerator;
using VerifyXunit;
/// <summary>
/// The generator_ test.
/// </summary>

[UsesVerify]
public class Generator_Test : CodeGeneratorSnapshotTest
{
    /// <summary>
    /// generator_simples the.
    /// </summary>
    /// <returns>A Task.</returns>
    [Fact]
    public Task generator_simple()
    {
        var source = """
            #nullable enable
            namespace Test;

            public class UserService
            {
                
            }
            #nullable restore
            """;

        return Verify(source, new ReflectCodeGenerator());
    }
}
