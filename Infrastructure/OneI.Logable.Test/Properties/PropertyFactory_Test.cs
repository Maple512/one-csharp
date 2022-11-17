namespace OneI.Logable.Properties;

using System.Text;

public class PropertyFactory_Test
{
    [Fact]
    public void create_null_value_factory()
    {
        var factory = new PropertyValueFactory();

        var nvalue = factory.Create((StringBuilder?)null);

        nvalue.ToString().ShouldBeEquivalentTo("null");

        var spv = factory.Create("这是一段字符串");

        spv.ToString().ShouldNotBeNull();

        var c = factory.Create<char>('1');

        c.ToString().ShouldBe("1");
    }
}
