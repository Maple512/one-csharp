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

using System;

[ToFastString]
public enum SomeEnum : byte
{
    A = 20, B, C, D, E, F
}

[ToFastString(HasDictionary = true)]
public enum SomeEnum1 : byte
{
    A = 20, B, C, F
}

[ToFastString(HasDictionary = true, DictionaryMethodName = "GetAAA")]
public enum SomeEnum2 : byte
{
    A = 20, B, C, D, E, F
}

[ToFastString("GETDDD",HasDictionary = true, DictionaryMethodName = "GetAAA")]
public enum SomeEnum3 : byte
{
    A = 20, B, C, D, E, F
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
