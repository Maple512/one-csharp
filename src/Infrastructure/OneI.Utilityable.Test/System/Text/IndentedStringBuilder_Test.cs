namespace System.Text;

public class IndentedStringBuilder_Test
{
    [Fact]
    public void indent_string()
    {
        var c = new IndentedStringBuilder();

        using(var _ = c.Indent())
        {
            c.Append('c');
            c.AppendLine();
        }

        c.AppendLine("123");

        var a = c.ToString();
    }
}
