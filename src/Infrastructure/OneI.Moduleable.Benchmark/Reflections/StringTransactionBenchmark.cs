namespace OneI.Moduleabl.Reflections;

public class StringTransactionBenchmark : BenchmarkItem
{
    [Params(1000)]
    public int length;

    [Benchmark(Baseline = true)]
    public void UseString()
    {
        for(var i = 0; i < length; i++)
        {
            Check.ThrowNullOrWhiteSpace(Randomizer.String(100));
        }
    }

    [Benchmark]
    public void UseSpan()
    {
        for(var i = 0; i < length; i++)
        {
            _ = CheckSt(Randomizer.String(100));
        }
    }

    bool CheckSt(scoped ReadOnlySpan<char> text)
    {
        if(text is not { Length: > 0 })
        {
            return false;
        }

        return text.IsWhiteSpace();
    }
}
