namespace OneI.Text;

public class ValueStringBulder_Test
{
    [Fact]
    public void print_size()
    {
        TestTools.PrintLayoutToFile<ValueStringBuilder>();
        TestTools.PrintLayoutToFile<StringBuilder>();
    }

    [Fact]
    public void format_string()
    {
        var builder = new ValueStringBuilder(256);

        builder.AppendFormat("{0}", "Maple512");

        builder.ToString().ShouldBe("");
    }

    [Fact]
    public void dispose_builder()
    {
        var builder = new ValueStringBuilder(256);

        builder.Append("message");

        builder.Dispose();
    }

    [Fact]
    public void span_formatter()
    {
        var builder = new ValueStringBuilder(256);

        builder.AppendFormat("{0}", 123);

        var a = builder.ToString();

        builder.Dispose();
    }
}
