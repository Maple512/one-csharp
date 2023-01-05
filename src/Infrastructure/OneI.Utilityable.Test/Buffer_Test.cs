namespace OneI;

using System;
using System.Text;
using ObjectLayoutInspector;
using OneT.Common;

public class Buffer_Test
{
    private const string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    static char[] Chars = text.ToCharArray();

    [Fact]
    public void operator_span()
    {
        Span<char> span = stackalloc char[text.Length + 10];

        text.CopyTo(span);

        span.ToString().ShouldBe("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789\0\0\0\0\0\0\0\0\0\0");

        span.Clear();

        span.ToString().ShouldBe(new string(char.MinValue, span.Length));
    }

    [Fact]
    public void simple()
    {
        var buffer = CharBuffer.Create(text);

        buffer.ToString().ShouldBe(text);

        buffer.ToArray().ShouldBe(Chars);
    }

    [Fact]
    public void size_print()
    {
        // size 只能是: 0、1、2、4、8、16、32、64或128
        Unsafe.SizeOf<CharBuffer>().ShouldBe(16);
        Unsafe.SizeOf<Memory<byte>>().ShouldBe(16);

        TestTools.PrintLayoutToFile<CharBuffer>();
        TestTools.PrintLayoutToFile<Memory<byte>>();
    }

    [Fact]
    public unsafe void buffer_test()
    {
        var array = Chars;

        var buffer = new CharBuffer(array);

        buffer.ToArray().ShouldBe(array);
        buffer.ToString().ShouldBe(text);

        var buffer1 = CharBuffer.Create(text.Length);

        buffer.TryCopyTo(buffer1).ShouldBeTrue();

        buffer1[..array.Length].ToArray().ShouldBe(array);
        buffer1[..array.Length].ToString().ShouldBe(text);
    }

    [Fact]
    public void buffer_create_from_string()
    {
        var text = "1234567890";

        CharBuffer.Create(text).ToString().ShouldBe(text);
    }

    [Fact]
    public void buffer_clear()
    {
        var buffer = CharBuffer.Create(text);

        buffer.ToString().ShouldBe(text);

        buffer.Clear();

        var array = buffer.ToArray();

        buffer.ToString().ShouldBe(string.Empty);
    }
}
