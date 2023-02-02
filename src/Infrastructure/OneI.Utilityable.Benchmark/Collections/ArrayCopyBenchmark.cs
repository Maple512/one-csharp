namespace OneI.Utilityable.Collections;

public class ArrayCopyBenchmark
{
    private const int count = 10;
    // 4
    [Benchmark(Baseline = true)]
    public void UseArray()
    {
        var a1 = new[] { 1, 2, 3, 4, 5, };
        var a2 = new int[a1.Length];

        for(var i = 0;i < count;i++)
        {
            Array.Copy(a1, a2, a2.Length);
        }
    }

    // 1
    [Benchmark]
    public void UseMemorySpan()
    {
        var a1 = new[] { 1, 2, 3, 4, 5, };
        var a2 = new int[a1.Length];

        for(var i = 0;i < count;i++)
        {
            a1.CopyTo(a2.AsSpan());
        }
    }
    // 3
    [Benchmark]
    public void UseMemoryMemory()
    {
        var a1 = new[] { 1, 2, 3, 4, 5, };
        var a2 = new int[a1.Length];

        for(var i = 0;i < count;i++)
        {
            a1.CopyTo(a2.AsMemory());
        }
    }
    //5
    [Benchmark]
    public void UseBufferBlockCopy()
    {
        var a1 = new[] { 1, 2, 3, 4, 5, };
        var a2 = new int[a1.Length];

        for(var i = 0;i < count;i++)
        {
            Buffer.BlockCopy(a1, 0, a2, 0, a2.Length * sizeof(int));
        }
    }

    // 2
    [Benchmark]
    public unsafe void UseBufferMemoryCopy()
    {
        var a1 = new[] { 1, 2, 3, 4, 5, };
        var a2 = new int[a1.Length];

        for(var i = 0;i < count;i++)
        {
            Buffer.MemoryCopy(
                Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(a1)),
                Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(a2)),
                a2.Length * sizeof(int),
                a1.Length * sizeof(int));
        }
    }
}
