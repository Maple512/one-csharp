namespace OneI.Utilityable;

public class HashCodeBenchmark
{
    [Params(1000)]
    public int length;

    [Benchmark(Baseline = true)]
    public void UseDefault()
    {
        for(int i = 0; i < length; i++)
        {
            new object().GetHashCode();
        }
    }

    [Benchmark]
    public void UseHashCode()
    {
        for(int i = 0; i < length; i++)
        {
            HashCode.Combine(new object());
        }
    }

    [Benchmark]
    public void UseEqualityComparer()
    {
        for(int i = 0; i < length; i++)
        {
            EqualityComparer<object>.Default.GetHashCode(new object());
        }
    }
}
