namespace OneI.Utilityable.Strings;

using Cysharp.Text;

public class StringReplaceBenchmark : BenchmarkItem
{
    [Params(100)]
    public int length;

    [Benchmark]
    public void UseDefault()
    {
        var text = Randomizer.String(100);

        for(var i = 0; i < length; i++)
        {
            _ = text.Replace("\"", "\\\"");
        }
    }

    [Benchmark]
    public void UseZString()
    {
        var text = Randomizer.String(100);
        var builder = ZString.CreateStringBuilder(true);

        for(var i = 0; i < length; i++)
        {
            builder.Replace("\"", "\\\"");

            _ = builder.ToString();

            builder.Clear();
        }

        builder.Dispose();
    }
}
