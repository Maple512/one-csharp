namespace OneI.Utilityable;

using System.Buffers;

public class TextWriterSameCharBenchmark : IValidator
{
    private const int count = 100;

    [Benchmark(Baseline = true)]
    public string UseString()
    {
        var writer = new StringWriter();

        for(int i = 0; i < 100; i++)
        {
            writer.Write(new string(' ', count));
        }
        return writer.ToString();
    }

    [Benchmark]
    public string UseSpan()
    {
        var writer = new StringWriter();

        for(int i = 0; i < 100; i++)
        {
            Span<char> span = stackalloc char[count];

            span.Fill(' ');

            writer.Write(span);

            span.Clear();
        }
        return writer.ToString();
    }

    public void Validate()
    {
        var result = UseString();

        IValidator.AreEquals(result, UseSpan());
    }
}
