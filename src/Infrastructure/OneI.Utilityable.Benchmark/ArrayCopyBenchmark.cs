namespace OneI.Utilityable;

using DotNext;
using DotNext.Runtime;

public unsafe class ArrayCopyBenchmark
{
    private const string text = "1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz";
    private static readonly int length = text.Length;
    private static readonly int bitSize = sizeof(char);
    private static readonly char[] Text1 = text.ToCharArray();
    private static readonly char[] Text2 = text.ToCharArray();
    private static readonly char[] Text3 = text.ToCharArray();
    private static readonly char[] Text4 = text.ToCharArray();

    [GlobalSetup]
    public void Check()
    {
        var array = text.ToCharArray();

        ThrowIfFalse(UseUnsafe().BitwiseEquals(array));

        ThrowIfFalse(UseMemory().BitwiseEquals(array));

        ThrowIfFalse(UseSpan().BitwiseEquals(array));

        ThrowIfFalse(UseArray().BitwiseEquals(array));
    }

    [Benchmark(Baseline = true)]
    public char[] UseArray()
    {
        var chars = new char[length];

        Text1.CopyTo(chars, 0);

        return chars;
    }

    [Benchmark]
    public char[] UseUnsafe()
    {
        var chars = new char[length];

        fixed(char* dest = chars)
        fixed(char* source = &Text2[0])
        {
            Unsafe.CopyBlockUnaligned(dest, source, (uint)(length * bitSize));
        }

        return chars;
    }

    [Benchmark]
    public char[] UseMemory()
    {
        var chars = new char[length];
        var len = (uint)(length * bitSize);
        fixed(char* dest = chars)
        fixed(char* source = &Text3[0])
        {
            Buffer.MemoryCopy(source, dest, len, len);
        }

        return chars;
    }

    [Benchmark]
    public char[] UseSpan()
    {
        var chars = new char[length];

        var span = new Span<char>(Text4);

        span.CopyTo(chars);

        return chars;
    }

    private static void ThrowIfFalse(bool condition, [CallerLineNumber] int? line = null)
    {
        if(condition == false)
        {
            throw new Exception(line!.Value.ToString());
        }
    }
}
