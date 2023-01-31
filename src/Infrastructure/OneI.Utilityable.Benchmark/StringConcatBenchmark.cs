namespace OneI.Utilityable;

using Cysharp.Text;
using DotNext;

public class StringConcatBenchmark : BenchmarkItem
{
    [Params(100)]
    public int length;

    public override void GlobalInlitialize()
    {
        UseDefault();
        UseDefault_AsSpan();
        UseZString();
        UseDotNext();
    }

    [Benchmark(Baseline = true)]
    public void UseDefault()
    {
        var s1 = Randomizer.String(10);
        var s2 = Randomizer.String(10);
        var s3 = Randomizer.String(10);

        for(var i = 0; i < length; i++)
        {
            _ = string.Concat(s1, s2, s3);
        }
    }

    [Benchmark]
    public void UseDefault_AsSpan()
    {
        var s1 = Randomizer.String(10).AsSpan();
        var s2 = Randomizer.String(10).AsSpan();
        var s3 = Randomizer.String(10).AsSpan();

        for(var i = 0; i < length; i++)
        {
            _ = string.Concat(s1, s2, s3);
        }
    }

    [Benchmark]
    public void UseZString()
    {
        var s1 = Randomizer.String(10);
        var s2 = Randomizer.String(10);
        var s3 = Randomizer.String(10);

        for(var i = 0; i < length; i++)
        {
            _ = ZString.Concat(s1, s2, s3);
        }
    }

    [Benchmark]
    public void UseDotNext()
    {
        var s1 = Randomizer.String(10).AsSpan();
        var s2 = Randomizer.String(10).AsSpan();
        var s3 = Randomizer.String(10).AsSpan();

        for(var i = 0; i < length; i++)
        {
            _ = s1.Concat(s2.Concat(s3).Span);
        }
    }
}
