namespace OneI.Logable.Internal;

using System.Collections.Generic;
using OneI.Logable.Parsing;
using OneI.Logable.Properties;

public class MessageResolver
{
    private readonly PropertyBinder _propertyBinder;

    public MessageResolver(IPropertyValueFactory propertyValueFactory)
    {
        _propertyBinder = new PropertyBinder(propertyValueFactory);
    }

    public void Resolve(
        string message,
        object?[] parameters,
        out IReadOnlyList<Token> tokens,
        out IReadOnlyList<Property> properties)
    {
        tokens = TextParser.Parse(message);

        properties = _propertyBinder.ConstructProperties(tokens, parameters);
    }
}
