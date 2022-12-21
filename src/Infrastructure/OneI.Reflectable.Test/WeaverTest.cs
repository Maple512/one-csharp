namespace OneI.Reflectable;

public class WeaverTest
{
    private static readonly TestResult testResult;

    static WeaverTest()
    {
        var weavingTask = new ModuleWeaver();

        testResult = weavingTask.ExecuteTestRun("OneI.Reflectable.Fody.TestAssembly.dll");
    }

    [Fact]
    public void validate_hello_world_is_injected()
    {
        var type = testResult.Assembly.GetType("TheNamespace.Hello");

        var instance = (dynamic)Activator.CreateInstance(type!)!;

        Assert.Equal("Hello World", instance.World());
    }
}
