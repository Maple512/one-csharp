namespace OneI.Utilityable;

using DotNext;
using ValueBuffer = ValueBuffer<char>;

public class StringToCharArray
{
    public const string str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    [GlobalSetup]
    public void Check()
    {
        var array = UseDefault();

        ThrowIfFalse(UseSpan().BitwiseEquals(array));

        ThrowIfFalse(UseCharBuffer().BitwiseEquals(array));
    }

    [Benchmark]
    public char[] UseDefault() => str.ToCharArray();

    [Benchmark]
    public char[] UseSpan() => str.AsSpan().ToArray();

    [Benchmark]
    public char[] UseCharBuffer() => ValueBuffer.Create(str).ToArray();

    static void ThrowIfFalse(bool condition)
    {
        if(condition == false)
        {
            throw new Exception();
        }
    }
}
