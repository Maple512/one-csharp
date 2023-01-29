namespace OneI.Logable;

using OneI.Logable.NLogInternal;
using Serilog.Parsing;
using Templates;

public class TemplateParserBenchmark
{
    private const string text = "a6sd5f4asdf45as6f{0:yyyy-MM-dd HH:mm:ss}asd6fa4sdf65asdf";

    [Params(1000)]
    public int count;

    [Benchmark(Baseline = true)]
    public void UseSeriLog()
    {
        for(var i = 0; i < count; i++)
        {
            var _ = new MessageTemplateParser().Parse(text);
        }
    }

    [Benchmark]

    public void UseLogable()
    {
        for(var i = 0; i < count; i++)
        {
            var _ = new TemplateEnumerator(text);
        }
    }

    [Benchmark]
    public void UseNLog()
    {
        for(var i = 0; i < count; i++)
        {
            var enumerator = new NLogTemplateEnumerator(text);
            while(enumerator.MoveNext())
            {
                _ = enumerator.Current;
            }
        }
    }
}
