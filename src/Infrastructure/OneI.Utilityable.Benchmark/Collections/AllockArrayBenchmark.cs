namespace OneI.Utilityable.Collections;

using System.Buffers;

public class AllockArrayBenchmark
{
    const int _capacity = 100;
    const int _count = 10;

    [Benchmark(Baseline = true)]
    public void UseList_Int()
    {
        for(var i = 0;i < _count;i++)
        {
            _ = new List<int>(_capacity);
        }
    }

    [Benchmark]
    public void UseArray()
    {
        for(var i = 0;i < _count;i++)
        {
            _ = new int[_capacity];
        }
    }

    [Benchmark]
    public void UseStackAlloc()
    {
        for(var i = 0;i < _count;i++)
        {
#pragma warning disable CA2014 // 不要循环使用 stackalloc
            _ = stackalloc int[_capacity];
#pragma warning restore CA2014 // 不要循环使用 stackalloc
        }
    }

    [Benchmark]
    public void UseArrayPool()
    {
        for(var i = 0;i < _count;i++)
        {
            _ = ArrayPool<int>.Shared.Rent(_capacity);
        }
    }

    [Benchmark]
    public void UseMemoryPool()
    {
        for(var i = 0;i < _count;i++)
        {
            _ = MemoryPool<int>.Shared.Rent(_capacity);
        }
    }

    [Benchmark]
    public void UseGCAllocate()
    {
        for(var i = 0;i < _count;i++)
        {
            _ = GC.AllocateArray<int>(_capacity);
        }
    }

    [Benchmark]
    public void UseGCAllocate_Pinned()
    {
        for(var i = 0;i < _count;i++)
        {
            _ = GC.AllocateArray<int>(_capacity,true);
        }
    }

    [Benchmark]
    public void UseGCAllocateUninitialized()
    {
        for(var i = 0;i < _count;i++)
        {
            _ = GC.AllocateUninitializedArray<int>(_capacity);
        }
    }

    [Benchmark]
    public void UseGCAllocateUninitialized_Pinned()
    {
        for(var i = 0;i < _count;i++)
        {
            _ = GC.AllocateUninitializedArray<int>(_capacity, true);
        }
    }
}
