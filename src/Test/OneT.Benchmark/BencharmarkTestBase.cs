namespace OneT.Benchmark;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Order;

[RankColumn, MemoryDiagnoser, NativeMemoryProfiler, ThreadingDiagnoser, DisassemblyDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest), MarkdownExporter]
public abstract class BencharmarkTestBase
{

}
