namespace OneT.CodeGenerator;

using System.Runtime.CompilerServices;
using VerifyTests;
/// <summary>
/// The test module initializer.
/// </summary>

public static class TestModuleInitializer
{
    /// <summary>
    /// Inits the.
    /// </summary>
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
