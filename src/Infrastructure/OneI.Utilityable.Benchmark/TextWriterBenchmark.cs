namespace OneI.Utilityable;

using Cysharp.Text;

public class TextWriterBenchmark
{
    public const int count = 50;

    [Params(50, 1000)]
    public int stringCount;

    [Params(50, 1000)]
    public int capacity;

    [Benchmark(Baseline = true)]
    public string UseStringWriter()
    {
        var StringValue = Randomizer.String(stringCount);

        var builder = new StringBuilder(capacity);

        using var writer = new StringWriter(builder);

        for(var i = 0; i < count; i++)
        {
            writer.Write(StringValue);

            writer.WriteLine();
        }

        return writer.ToString();
    }

    [Benchmark]
    public string UseValueStringWriter()
    {
        var StringValue = Randomizer.String(stringCount);

        using var writer = new ZStringWriter();

        for(var i = 0; i < count; i++)
        {
            writer.Write(StringValue);
            writer.WriteLine();
        }

        return writer.ToString();
    }
}
