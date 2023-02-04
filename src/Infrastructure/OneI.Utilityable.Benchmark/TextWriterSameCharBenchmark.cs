namespace OneI.Utilityable;

public class TextWriterSameCharBenchmark : BenchmarkItem
{
    private const int count = 100;

    [Benchmark(Baseline = true)]
    public string UseString()
    {
        var writer = new StringWriter();

        for(var i = 0; i < 100; i++)
        {
            writer.Write(new string(' ', count));
        }

        return writer.ToString();
    }

    [Benchmark]
    public string UseSpan()
    {
        var writer = new StringWriter();

        for(var i = 0; i < 100; i++)
        {
#pragma warning disable CA2014 // 不要循环使用 stackalloc
            Span<char> span = stackalloc char[count];
#pragma warning restore CA2014 // 不要循环使用 stackalloc

            span.Fill(' ');

            writer.Write(span);

            span.Clear();
        }

        return writer.ToString();
    }

    public override void Inlitialize()
    {
        var result = UseString();

        BenchmarkItem.AreEquals(result, UseSpan());
    }
}
