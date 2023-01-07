namespace OneI.Utilityable.Benchmark;

using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        // BenchmarkTool.RunAssymbly<Program>(args);

#if DEBUG
        new InternalMethodReflectionBenchmark().Validate();
#endif

        var s = IL.Fast(100);

        BenchmarkTool.Run<InternalMethodReflectionBenchmark>(args);
    }
}
