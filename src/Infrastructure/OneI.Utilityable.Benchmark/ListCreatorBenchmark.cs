namespace OneI.Utilityable;

using System.Buffers;

public class ListCreatorBenchmark
{
    const int _capacity = 100;
    const int _count = 10;

    [BenchmarkCategory("int")]
    [Benchmark(Baseline = true)]
    public void UseList_Int()
    {
        for(int i = 0; i < _count; i++)
        {
            _ = new List<int>(_capacity);
        }
    }

    [BenchmarkCategory("int")]
    [Benchmark]
    public void UseArray_Int()
    {
        for(int i = 0; i < _count; i++)
        {
            _ = new int[_capacity];
        }
    }

    [Benchmark]
    [BenchmarkCategory("int")]
    public void UseStackAlloc_Int()
    {
        for(int i = 0; i < _count; i++)
        {
#pragma warning disable CA2014 // 不要循环使用 stackalloc
            _ = stackalloc int[_capacity];
#pragma warning restore CA2014 // 不要循环使用 stackalloc
        }
    }

    [Benchmark]
    [BenchmarkCategory("int")]
    public void UseArrayPool_Int()
    {
        for(int i = 0; i < _count; i++)
        {
            _ = ArrayPool<int>.Shared.Rent(_capacity);
        }
    }

    [Benchmark]
    [BenchmarkCategory("int")]
    public void UseMemoryPool_Int()
    {
        for(int i = 0; i < _count; i++)
        {
            _ = MemoryPool<int>.Shared.Rent(_capacity);
        }
    }

    [BenchmarkCategory("class")]
    [Benchmark]
    public void UseList_Class()
    {
        for(int i = 0; i < _count; i++)
        {
            _ = new List<User>(_capacity);
        }
    }

    [BenchmarkCategory("class")]
    [Benchmark]
    public void UseArray_Class()
    {
        for(int i = 0; i < _count; i++)
        {
            _ = new User[_capacity];
        }
    }

    //    [Benchmark]
    //    [BenchmarkCategory("class")]
    //    public void UseStackAlloc_Int()
    //    {
    //        for(int i = 0; i < _count; i++)
    //        {
    //#pragma warning disable CA2014 // 不要循环使用 stackalloc
    //            _ = stackalloc User[_capacity];
    //#pragma warning restore CA2014 // 不要循环使用 stackalloc
    //        }
    //    }

    [Benchmark]
    [BenchmarkCategory("class")]
    public void UseArrayPool_Class()
    {
        for(int i = 0; i < _count; i++)
        {
            _ = ArrayPool<User>.Shared.Rent(_capacity);
        }
    }

    [Benchmark]
    [BenchmarkCategory("class")]
    public void UseMemoryPool_Class()
    {
        for(int i = 0; i < _count; i++)
        {
            _ = MemoryPool<User>.Shared.Rent(_capacity);
        }
    }

    [BenchmarkCategory("struct")]
    [Benchmark]
    public void UseList_Struct()
    {
        for(int i = 0; i < _count; i++)
        {
            _ = new List<UserStruct>(_capacity);
        }
    }

    [BenchmarkCategory("struct")]
    [Benchmark]
    public void UseArray_Struct()
    {
        for(int i = 0; i < _count; i++)
        {
            _ = new UserStruct[_capacity];
        }
    }

    [Benchmark]
    [BenchmarkCategory("struct")]
    public void UseStackAlloc_Struct()
    {
        for(int i = 0; i < _count; i++)
        {
#pragma warning disable CA2014 // 不要循环使用 stackalloc
            _ = stackalloc UserStruct[_capacity];
#pragma warning restore CA2014 // 不要循环使用 stackalloc
        }
    }

    [Benchmark]
    [BenchmarkCategory("struct")]
    public void UseArrayPool_Struct()
    {
        for(int i = 0; i < _count; i++)
        {
            _ = ArrayPool<UserStruct>.Shared.Rent(_capacity);
        }
    }

    [Benchmark]
    [BenchmarkCategory("struct")]
    public void UseMemoryPool_Struct()
    {
        for(int i = 0; i < _count; i++)
        {
            _ = MemoryPool<UserStruct>.Shared.Rent(_capacity);
        }
    }
}

public class User
{
    public int Id { get; set; }
}

public readonly struct UserStruct
{
    public readonly int _Id;
}
