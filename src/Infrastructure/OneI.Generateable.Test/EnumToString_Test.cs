namespace OneI.Generateable;

using OneT.CodeGenerator;

[UsesVerify]
public class EnumToString_Test : CodeGeneratorSnapshotTest
{
    [Fact]
    public Task generate_tofaststring_code()
    {
        var source =
"""
namespace OneI.Generateable;

using System;
using OneI.Generateable.CodeGenerated;

[ToFastString]
public enum SomeEnum 
{
    A , B, C, D, E, F
}
""";

        return Verify(source, new EnumFastToString.CodeGenerator());
    }
}

public enum SomeEnum
{
    A, B, C, D, E, F
}

public enum SomeEnum1 : byte
{
    A = 20, B, C, F
}

public enum SomeEnum2 : byte
{
    A = 20, B, C, D, E, F
}

public enum SomeEnum3 : byte
{
    A = 20, B, C, D, E, F
}
