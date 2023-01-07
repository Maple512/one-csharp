namespace System;

using System.Reflection;

public class String_Test
{
    [Fact]
    public void reflection_string()
    {
        var method = DotNext.Reflection.Type<string>.Method<int>.GetStatic<string>("FastAllocateString", true);

        var type = typeof(string);

        var methods = type.GetRuntimeMethods();
    }
}
