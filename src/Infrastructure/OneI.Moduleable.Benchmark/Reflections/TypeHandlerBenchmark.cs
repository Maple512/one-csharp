namespace OneI.Moduleabl.Reflections;

using DotNext.Runtime;

public class TypeHandlerBenchmark : BenchmarkItem
{
    [Params(1000)]
    public int length;

    private static object _object;

    public override void Inlitialize()
    {
        _object = new object();
    }

    [Benchmark(Baseline = true)]
    public void UseTypeOf()
    {
        for(var i = 0; i < length; i++)
        {
            _ = typeof(object).TypeHandle;
        }
    }

    [Benchmark]
    public void UseGetType()
    {
        for(var i = 0; i < length; i++)
        {
            _ = _object.GetType().TypeHandle;
        }
    }

    [Benchmark]
    public void UseDotNext()
    {
        for(var i = 0; i < length; i++)
        {
            _ = Intrinsics.TypeOf<object>();
        }
    }

    [Benchmark]
    public void UseIL()
    {
        for(var i = 0; i < length; i++)
        {
            _ = IL.GetTypeHandle<object>();
        }
    }
}
