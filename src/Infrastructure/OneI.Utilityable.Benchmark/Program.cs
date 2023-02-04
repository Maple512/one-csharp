namespace OneI.Utilityable.Benchmark;

using BenchmarkDotNet.Configs;
using OneI.Utilityable.Strings;

internal class Program
{
    private static void Main(string[] args)
    {
        // BenchmarkTool.RunAssymbly<Program>(args);

        BenchmarkTool.Run<StringEscapeBenchmark>(args);
    }
}
