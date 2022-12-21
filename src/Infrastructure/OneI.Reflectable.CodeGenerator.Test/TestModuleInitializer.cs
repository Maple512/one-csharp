namespace OneI.Reflectable;

using VerifyTests;

/// <summary>
/// The test module initializer.
/// </summary>
public class TestModuleInitializer
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
