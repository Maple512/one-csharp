namespace OneI.Utilityable.Benchmark;

using BenchmarkDotNet.Configs;

internal class Program
{
    private static void Main(string[] args)
    {
        // BenchmarkTool.RunAssymbly<Program>(args);

        BenchmarkTool.Run<DictionaryCacheBenchmark>(args);
    }
}
