namespace OneI.Utilityable.Collections;

public class ForeachBenchmark : BenchmarkItem
{
    [Params(1000)]
    public int length;

    private List<Model1> list1;
    private List<Model2> list2;

    public override void Inlitialize()
    {
        list1 = AutoFaker.Generate<Model1>(length);
        list2 = AutoFaker.Generate<Model2>(length);
    }

    // 1
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void UseFor()
    {
        for(var i = 0; i < length; i++)
        {
            Print(list1[i].id);
        }
    }

    // 2
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void UseForeach()
    {
        foreach(var item in list1)
        {
            Print(item.id);
        }
    }

    // 2
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void UseForeach_Dispose()
    {
        foreach(var item in list2)
        {
            Print(item.id);
        }
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    void Print(int id)
    {
        var a = id;
    }

    private class Model1
    {
        public int id;

        public string name;
    }

    private class Model2 : IDisposable
    {
        private bool _disposed;

        public int id;

        public string name;

        public void Dispose()
        {
            if(_disposed)
            {
                throw new ObjectDisposedException(nameof(Model2));
            }

            _disposed = true;
        }
    }
}
