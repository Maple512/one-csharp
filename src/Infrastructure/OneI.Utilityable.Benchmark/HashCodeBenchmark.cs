namespace OneI.Utilityable;

public class HashCodeBenchmark
{
    [Params(1000)]
    public int length;

    [Benchmark(Baseline = true)]
    public void UseDefault()
    {
        for(var i = 0; i < length; i++)
        {
            _ = new object().GetHashCode();
        }
    }

    [Benchmark]
    public void UseHashCode()
    {
        for(var i = 0; i < length; i++)
        {
            _ = HashCode.Combine(new object());
        }
    }

    [Benchmark]
    public void UseEqualityComparer()
    {
        for(var i = 0; i < length; i++)
        {
            _ = EqualityComparer<object>.Default.GetHashCode(new object());
        }
    }
}
