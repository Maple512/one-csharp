namespace OneI.Utilityable;

using System;
using DotNext;

public class ArrayClearBenchmark
{
    const string text = "1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz";
    static char[] Text1 = text.ToCharArray();
    static char[] Text2 = text.ToCharArray();
    static char[] Text3 = text.ToCharArray();

    [GlobalSetup]
    public void Check()
    {
        var empty = new char[text.Length];

        UseArray();
        ThrowIfFalse(Text1.BitwiseEquals(empty));

        UseSpan();
        ThrowIfFalse(Text2.BitwiseEquals(empty));

        UseCharBuffer();
        ThrowIfFalse(Text3.BitwiseEquals(empty));
    }

    [Benchmark(Baseline = true)]
    public void UseArray()
    {
        Array.Clear(Text1);
    }

    [Benchmark]
    public void UseSpan()
    {
        var span = new Span<char>(Text2);

        span.Clear();
    }

    [Benchmark]
    public void UseCharBuffer()
    {
        var buffer = new ByteBuffer(Text3);

        buffer.Clear();
    }

    static void ThrowIfFalse(bool condition, [CallerLineNumber] int? line = null)
    {
        if(condition == false)
        {
            throw new Exception(line!.Value.ToString());
        }
    }
}
