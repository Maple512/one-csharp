namespace OneI.Utilityable;

public class SpanBenchmark
{
    private const int count = 100;

    // 1
    [Benchmark(Baseline = true)]
    public void UseIndexOf()
    {
        var array = Enumerable.Range(0, count).ToArray().AsSpan();

        for(var i = 0; i < count; i++)
        {
            _ = array.IndexOf(-1) != -1;
        }
    }
    // 2
    [Benchmark]
    public void UseContains()
    {
        var array = Enumerable.Range(0, count).ToArray().AsSpan();

        for(var i = 0; i < count; i++)
        {
            _ = array.Contains(-1);
        }
    }
}
