namespace OneI.Utilityable.Benchmark;

using OneI.Utilityable.Strings;

internal class Program
{
    private static void Main(string[] args)
    {
        // BenchmarkTool.RunAssymbly<Program>(args);

        BenchmarkTool.Run<StringEscapeBenchmark>(args);
    }
}
