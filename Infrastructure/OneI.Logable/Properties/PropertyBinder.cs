namespace OneI.Logable.Properties;

using System;
using System.Collections.Generic;
using OneI.Logable.Parsing;

public class PropertyBinder
{
    private readonly IPropertyValueFactory _propertyValueFactory;

    public PropertyBinder(IPropertyValueFactory propertyValueFactory)
    {
        _propertyValueFactory = propertyValueFactory;
    }

    public IReadOnlyList<Property> ConstructProperties(IReadOnlyList<Token> tokens, object?[]? parameters)
    {
        if(parameters.IsNullOrEmpty())
        {
            return Array.Empty<Property>();
        }

        var result = new Property[tokens.Count];

        var minCount = Math.Min(tokens.Count, parameters.Length);

        var index = 0;
        foreach(var token in tokens)
        {
            if(token is PropertyToken propertyToken)
            {
                var deconstruct = propertyToken.ParsingType == PropertyTokenType.Deconstruct;
                object? parameter = null;

                // 获取指定索引的参数
                if(parameters.Length > propertyToken.ParameterIndex)
                {
                    parameter = parameters[propertyToken.ParameterIndex];
                }
                else
                {
                    parameter = propertyToken.Text;
                }

                result[index] = new(propertyToken.Name, _propertyValueFactory.Create(parameter, deconstruct));
            }
            else
            {
                result[index] = new(token.Text, _propertyValueFactory.Create(token.Text));
            }

            index++;
        }

        return result;
    }
}
