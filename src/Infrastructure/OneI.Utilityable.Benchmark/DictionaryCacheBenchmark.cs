namespace OneI.Utilityable;

public class DictionaryCacheBenchmark
{
    private const int length = 10;

    public string[] StringValue = { "", "abccdah87jwgr", "wfjwkhwfhgeg", "wfjwkhugteg", "wfjwkhugteg", "wfjwkhugteg", "wfjwkhwfhgewjugteg" };
    //2
    [Benchmark(Baseline = true)]
    public void UseString_Default()
    {
        Dictionary<string, int> _cache = new(5);

        for(var i = 0; i < length; i++)
        {
            foreach(var item in StringValue)
            {
                _cache.GetOrAdd(item, item.GetHashCode());
            }
        }
    }
    //3
    [Benchmark]
    public void UseString_EqualityComparer()
    {
        Dictionary<string, int> _cache = new(5, EqualityComparer<string>.Default);

        for(var i = 0; i < length; i++)
        {
            foreach(var item in StringValue)
            {
                _cache.GetOrAdd(item, item.GetHashCode());
            }
        }
    }
    //4
    [Benchmark]
    public void UseHashCode_Default()
    {
        Dictionary<int, int> _cache = new(5);

        for(var i = 0; i < length; i++)
        {
            foreach(var item in StringValue)
            {
                _cache.GetOrAdd(item.GetHashCode(), item.GetHashCode());
            }
        }
    }
    //1
    [Benchmark]
    public void UseHashCode_EqualityComparer()
    {
        Dictionary<int, string> _cache = new(5, EqualityComparer<int>.Default);

        for(var i = 0; i < length; i++)
        {
            foreach(var item in StringValue)
            {
                _cache.GetOrAdd(item.GetHashCode(), item);
            }
        }
    }
}
