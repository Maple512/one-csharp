namespace OneI.Utilityable;

using DotNext;

public class ArraySliceBenchmark
{
    private const string text = "1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz";
    const int start = 3;
    const int end = 10;
    private static readonly char[] Text1 = text.ToCharArray();
    private static readonly char[] Text2 = text.ToCharArray();
    private static readonly char[] Text3 = text.ToCharArray();

    [GlobalSetup]
    public void Check()
    {
        var array = text[start..end].ToCharArray();

        ThrowIfFalse(UseSpan().BitwiseEquals(array));

        ThrowIfFalse(UseCharBuffer().BitwiseEquals(array));

        ThrowIfFalse(UseArray().BitwiseEquals(array));
    }

    [Benchmark(Baseline = true)]
    public char[] UseArray()
    {
        return Text1[start..end];
    }

    [Benchmark]
    public char[] UseSpan()
    {
        var span = new Span<char>(Text2);

        return span[start..end].ToArray();
    }

    [Benchmark]
    public char[] UseCharBuffer()
    {
        var span = new ByteBuffer(Text3);

        return span[start..end].ToArray();
    }

    private static void ThrowIfFalse(bool condition, [CallerLineNumber] int? line = null)
    {
        if(condition == false)
        {
            throw new Exception(line!.Value.ToString());
        }
    }
}
