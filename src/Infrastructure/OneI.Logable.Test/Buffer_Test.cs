namespace OneI.Logable;

using System.Buffers;
using System.Text;
using DotNext.Buffers;

public class Buffer_Test
{
    [Fact]
    public void buffer()
    {
        using var writer = new PooledArrayBufferWriter<byte> { BufferPool = ArrayPool<byte>.Shared };
        writer.Write(Encoding.UTF8.GetBytes("message1"));
        writer.Write(Encoding.UTF8.GetBytes("message2"));

        var aa = Encoding.UTF8.GetString(writer.ToArray());

        Memory<byte> memory = Encoding.UTF8.GetBytes("Message");

        ((ReadOnlySpan<byte>)Encoding.UTF8.GetBytes("span")).CopyTo(memory.Span);

        var result = Encoding.UTF8.GetString(memory.Span);
    }
}
