namespace OneI.Utilityable;

using System.Reflection;

public class InternalMethodReflectionBenchmark
{
    const string MethodName = "FastAllocateString";

#if DEBUG
    internal void Validate()
    {
        UseReflection();

        UseDotNext();
    }
#endif

    [Benchmark]
    public MethodInfo UseReflection()
    {
        var method = typeof(string).GetMethod(MethodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);

        return method ?? throw new ArgumentNullException();
    }

    [Benchmark]
    public MethodInfo UseDotNext()
    {
        return DotNext.Reflection.Type<string>.Method<int>.RequireStatic<string>(MethodName, true);
    }
}
