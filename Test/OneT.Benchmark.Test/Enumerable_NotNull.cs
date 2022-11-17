namespace OneT.Benchmark;

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

[MemoryDiagnoser, RankColumn, NativeMemoryProfiler, DisassemblyDiagnoser, MinColumn, MaxColumn]
public class Enumerable_NotNull
{
    private static readonly IEnumerable<int> _array = Enumerable.Range(0, 100).ToList();

    // rank: 1
    [Benchmark(Baseline = true)]
    public void Any() => _array.Any();

    // rank: 2
    [Benchmark]
    public void First() => _array.First();

    // rank: 3
    [Benchmark]
    public void FirstOrDefault() => _array.FirstOrDefault();
}
