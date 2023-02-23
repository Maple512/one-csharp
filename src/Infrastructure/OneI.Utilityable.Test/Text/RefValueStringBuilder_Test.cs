namespace OneI.Text;

public sealed class RefValueStringBuilder_Test
{
    [Theory]
    [InlineData("999999999", '9', '0')]
    [InlineData("999999999", '0', '9')]
    [InlineData("啊手动阀手我阀骄傲", '我', '他')]
    public void replace_char(string text, char old, char @new)
    {
        var builder = new ValueStringBuilder(stackalloc char[512]);
        builder.Append(text);

        builder.Replace(old, @new);

        builder.ToString().ShouldBe(text.Replace(old, @new));
    }
}
