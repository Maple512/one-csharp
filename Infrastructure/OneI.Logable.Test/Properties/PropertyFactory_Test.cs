namespace OneI.Logable.Properties;

using System;
using System.IO;
using System.Linq;
using OneI.Logable.Properties.Policies;

public class PropertyFactory_Test
{
    [Fact]
    public void create_null_value_factory()
    {
        var factory = new PropertyValueFactory(
            Enumerable.Empty<Type>(),
            Enumerable.Empty<IDestructuringPolicy>(),
            false,
            10,
            1024,
            10);

        var nvalue = factory.Create(TestHelpler.CreateNewUser(), true);

        var a = nvalue.ToString();

        var output = new StringWriter();

        nvalue.Render(output);

        var result = output.ToString();
    }
}
