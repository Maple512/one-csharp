namespace OneI.Reflectable;

using VerifyTests;

public static class TestModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
