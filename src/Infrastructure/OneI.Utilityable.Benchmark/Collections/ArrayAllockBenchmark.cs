namespace OneI.Utilityable.Collections;

using System.Buffers;

public class ArrayAllockBenchmark
{
    private const int _capacity = 4096;
    private const int _count = 10;

    [Benchmark(Baseline = true)]
    public void UseList_Int()
    {
        for(var i = 0; i < _count; i++)
        {
            _ = new List<int>(_capacity);
        }
    }

    [Benchmark]
    public void UseArray()
    {
        for(var i = 0; i < _count; i++)
        {
            _ = new byte[_capacity];
        }
    }

    [Benchmark]
    public void UseStackAlloc()
    {
        for(var i = 0; i < _count; i++)
        {
#pragma warning disable CA2014 // 不要循环使用 stackalloc
            _ = stackalloc byte[_capacity];
#pragma warning restore CA2014 // 不要循环使用 stackalloc
        }
    }

    [Benchmark]
    public void UseArrayPool()
    {
        for(var i = 0; i < _count; i++)
        {
            var array = ArrayPool<byte>.Shared.Rent(_capacity);

            ArrayPool<byte>.Shared.Return(array);
        }
    }

    [Benchmark]
    public void UseMemoryPool()
    {
        for(var i = 0; i < _count; i++)
        {
            var array = MemoryPool<byte>.Shared.Rent(_capacity);

            array.Dispose();
        }
    }

    [Benchmark]
    public void UseGCAllocate()
    {
        for(var i = 0; i < _count; i++)
        {
            var array = GC.AllocateArray<byte>(_capacity);
        }
    }

    [Benchmark]
    public void UseGCAllocate_Pinned()
    {
        for(var i = 0; i < _count; i++)
        {
            var array = GC.AllocateArray<byte>(_capacity, true);
        }
    }

    [Benchmark]
    public void UseGCAllocateUninitialized()
    {
        for(var i = 0; i < _count; i++)
        {
            var array = GC.AllocateUninitializedArray<byte>(_capacity);
        }
    }

    [Benchmark]
    public void UseGCAllocateUninitialized_Pinned()
    {
        for(var i = 0; i < _count; i++)
        {
            var array = GC.AllocateUninitializedArray<byte>(_capacity, true);
        }
    }
}
