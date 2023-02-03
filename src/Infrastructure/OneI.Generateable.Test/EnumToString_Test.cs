namespace OneI.Generateable;

using OneT.CodeGenerator;

[UsesVerify]
public class EnumToString_Test : CodeGeneratorSnapshotTest
{
    [Fact]
    public Task generator_simple()
    {
        var source =
"""
namespace OneI.Generateable;

[ToFastString("ToSomeEnum")]
public enum SomeEnum
{
    A, B, C, D, E, F
}
""";

        return Verify(source, new EnumFastToString.CodeGenerator());
    }
}

public enum SomeEnum
{
    A, B, C, D, E, F
}
