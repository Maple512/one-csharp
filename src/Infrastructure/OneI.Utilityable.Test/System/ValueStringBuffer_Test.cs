
namespace System;

using OneI;
using OneT.Common;

public class ValueStringBuffer_Test
{
    [Fact]
    public void show_layout()
    {
        TestTools.PrintLayoutToFile<ValueBuffer>();
    }

    [Fact]
    public void set_string()
    {
        var buffer = new ValueBuffer();

        buffer.Append("m");
        buffer.Append("me");
        buffer.Append("m651651123");

        var result = buffer.ToString();

        result.ShouldBe("mmem651651123");

        buffer.Append(Randomizer.String(253));

        result = buffer.ToStringAndClear();

        result.Length.ShouldBe(266);
    }

    [Fact]
    public void multiplo_cleanups_must_throw_exceptions()
    {
        var handler = new ValueBuffer();

        Should.NotThrow(() => handler.ToStringAndClear());

        Should.Throw<InvalidOperationException>(() => handler.ToStringAndClear());
    }
}
