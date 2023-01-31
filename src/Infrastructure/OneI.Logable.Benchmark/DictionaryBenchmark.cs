namespace OneI.Logable;

using OneI.Logable.Templates;

public class DictionaryBenchmark : BenchmarkItem
{
    [Params(100)]
    public  int count ;

    [Params(20)]
    public  int capacity;

    public override void GlobalInlitialize()
    {
        UseDictionry();

        UseValueDictionary();
    }

    [Benchmark(Baseline = true)]
    public void UseDictionry()
    {
        for (var i = 0; i < count; i++)
        {
            var dic = new Dictionary<int, int>(capacity);
            _ = dic.TryAdd(i, i);
            _ = dic.TryGetValue(i, out var value);

            if (dic.ContainsKey(i) == false
                || value != i)
            {
                throw new Exception();
            }
        }
    }

    [Benchmark]
    public void UseValueDictionary()
    {
        for (var i = 0; i < count; i++)
        {
            var dic = new PropertyDictionary<int, int>();
            _ = dic.TryAdd(i, i);
            _ = dic.TryGetValue(i, out var value);

            if (dic.ContainsKey(i) == false
                || value != i)
            {
                throw new Exception();
            }

            dic.Dispose();
        }
    }
}
