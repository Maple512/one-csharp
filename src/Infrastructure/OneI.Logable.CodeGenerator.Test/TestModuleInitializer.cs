namespace OneI.Logable;

public static class TestModuleInitializer
{
    [ModuleInitializer]
    public static void Init() => VerifySourceGenerators.Enable();
}
