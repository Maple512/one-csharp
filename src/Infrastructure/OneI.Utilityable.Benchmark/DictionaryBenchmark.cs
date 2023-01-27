namespace OneI.Utilityable;

public class DictionaryBenchmark
{
    private const int count = 100;
    private const int capacity = 20;

    [Benchmark(Baseline = true)]
    public void UseDictionry()
    {
        for(var i = 0; i < count; i++)
        {
            var dic = new Dictionary<int, int>(capacity);

            dic.TryAdd(i, i);

            dic.TryGetValue(i, out var value);

            if(dic.ContainsKey(i) == false
                || value != i)
            {
                throw new Exception();
            }
        }
    }

    [Benchmark]
    public void UseValueDictionary()
    {
        for(var i = 0; i < count; i++)
        {
            var dic = new ValueDictionary<int, int>(capacity);

            dic.TryAdd(i, i);

            dic.TryGetValue(i, out var value);

            if(dic.ContainsKey(i) == false
                || value != i)
            {
                throw new Exception();
            }
        }
    }
}
