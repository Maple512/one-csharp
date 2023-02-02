namespace OneT.CodeGenerator;

/// <summary>
///     The test module initializer.
/// </summary>
public static class TestModuleInitializer
{
    /// <summary>
    ///     Inits the.
    /// </summary>
    [ModuleInitializer]
    public static void Init() => VerifySourceGenerators.Enable();
}
