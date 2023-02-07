namespace System;

using DotNext.Reflection;
using Reflection;

public class String_Test
{
    [Fact]
    public void reflection_string()
    {
        _ = Type<string>.Method<int>.GetStatic<string>("FastAllocateString", true);

        var type = typeof(string);
        _ = type.GetRuntimeMethods();
    }
}
