namespace OneI.Generateable;

public static class TestModuleInitializer
{
    [ModuleInitializer]
    public static void Init() => VerifySourceGenerators.Enable();
}
