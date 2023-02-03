namespace OneI.Utilityable.Collections;

public class ArrayClearBenchmark
{
    private const int count = 10;

    [Benchmark(Baseline = true)]
    public void UseArray()
    {
        for(var i = 0; i < count; i++)
        {
            var a1 = new[] { 1, 2, 3, 4, 5, };

            Array.Clear(a1);
        }
    }

    [Benchmark]
    public void UseMemorySpan()
    {
        for(var i = 0; i < count; i++)
        {
            var a1 = new[] { 1, 2, 3, 4, 5, };

            a1.AsSpan().Clear();
        }
    }
}
