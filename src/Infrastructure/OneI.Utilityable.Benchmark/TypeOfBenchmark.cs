namespace OneI.Utilityable;

public class TypeOfBenchmark
{
    [Params(1000)]
    public int count;

    //2
    [Benchmark(Baseline = true)]
    public void UseGetType()
    {
        for(var i = 0; i < count; i++)
        {
            _ = new object().GetType();
        }
    }
    //1
    [Benchmark]
    public void UseTypeOf()
    {
        for(var i = 0; i < count; i++)
        {
            _ = typeof(object);
        }
    }

    //1
    [Benchmark]
    public void UseDotNext()
    {
        for(var i = 0; i < count; i++)
        {
            _ = DotNext.Runtime.Intrinsics.TypeOf<object>();
        }
    }
}
