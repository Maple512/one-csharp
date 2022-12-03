namespace OneT.CodeGenerator;

using System.Runtime.CompilerServices;
using VerifyTests;

public static class TestModuleInitializer
{
    [ModuleInitializer]
    public static void Init() => VerifySourceGenerators.Enable();
}
