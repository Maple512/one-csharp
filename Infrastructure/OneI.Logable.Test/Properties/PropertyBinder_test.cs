namespace OneI.Logable.Properties;

using System;
using System.Linq;
using OneI.Logable.Parsing;
using OneI.Logable.Properties.Policies;

public class PropertyBinder_test
{
    [Fact]
    public void aaaaaaaaa()
    {
        var users = TestHelpler.CreateNewUser();

        var binder = new PropertyBinder(new PropertyValueFactory(
            Enumerable.Empty<Type>(),
            Enumerable.Empty<IDestructuringPolicy>(),
            false,
            10,
            1024,
            10));

        var tokens = TextParser.Parse("a {@User} {0}{3}");

        var parameters = new object[] { users };

        var properties = binder.ConstructProperties(tokens.ToList(), parameters);
    }
}
