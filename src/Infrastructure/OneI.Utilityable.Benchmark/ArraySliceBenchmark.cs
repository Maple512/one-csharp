namespace OneI.Utilityable;

public class ArraySliceBenchmark
{
    private const int count = 100;

    private readonly int capacity;
    // 4
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Array Slice")]
    public void UseArray()
    {
        var half = capacity / 2;
        var array = Randomizer.String(capacity).ToCharArray();

        for(var i = 0; i < count; i++)
        {
            var b = array[..half];
        }
    }
    //2
    [Benchmark]
    [BenchmarkCategory("Array Slice")]
    public void UseSpanSlice()
    {
        var half = capacity / 2;
        var array = Randomizer.String(capacity).ToCharArray();

        for(var i = 0; i < count; i++)
        {
            var b = array.AsSpan()[..half];
        }
    }
    // 5
    [Benchmark]
    [BenchmarkCategory("Array Slice")]
    public void UseArraySlice()
    {
        var half = capacity / 2;
        var array = Randomizer.String(capacity).ToCharArray();

        for(var i = 0; i < count; i++)
        {
            var b = array[..half].AsSpan();
        }
    }
    //3
    [Benchmark]
    [BenchmarkCategory("Array Slice")]
    public void UseStringSlice()
    {
        var half = capacity / 2;
        var array = Randomizer.String(capacity);

        for(var i = 0; i < count; i++)
        {
            var b = array[..half].AsSpan();
        }
    }
    //1
    [Benchmark]
    [BenchmarkCategory("Array Slice")]
    public void UseStringSpanSlice()
    {
        var half = capacity / 2;
        var array = Randomizer.String(capacity);

        for(var i = 0; i < count; i++)
        {
            var b = array.AsSpan()[..half];
        }
    }
    // 1
    [Benchmark]
    [BenchmarkCategory("Create String")]
    public void UseNewString()
    {
        var half = capacity / 2;
        var array = Randomizer.String(capacity).ToCharArray();

        for(var i = 0; i < count; i++)
        {
            var b = new string(array);
        }
    }
    // 2
    [Benchmark]
    [BenchmarkCategory("Create String")]
    public void UseAsSpanNewString()
    {
        var half = capacity / 2;
        var array = Randomizer.String(capacity).ToCharArray();

        for(var i = 0; i < count; i++)
        {
            var b = new string(array.AsSpan());
        }
    }
    //3
    [Benchmark]
    [BenchmarkCategory("Create String")]
    public void UseSpanToString()
    {
        var half = capacity / 2;
        var array = Randomizer.String(capacity).ToCharArray();

        for(var i = 0; i < count; i++)
        {
            var b = array.AsSpan().ToString();
        }
    }
}
