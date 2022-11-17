namespace OneI.Logable.Properties;

using System;
using OneI.Logable.Properties.Internal;

public class PropertyValueFactory : IPropertyValueFactory
{
    public IPropertyValue Create<TValue>(TValue? value)
    {
        if(value is null)
        {
            return NullPropertyValue.Instance;
        }

        if(value is string str)
        {
            return new StringPropertyValue(str);
        }

        if(value is ValueType)
        {
            var valueType = typeof(TValue);

            return value switch
            {
                // todo：这里需要改，创建
                bool or char or sbyte or byte or short or ushort or int or uint or long or ulong or float or double
                or decimal or DateTime or DateOnly or DateTimeOffset or TimeOnly or TimeSpan or Guid
                => (IPropertyValue)Activator.CreateInstance(typeof(PrimitivePropertyValue<>).MakeGenericType(valueType), value)!,
                _ => (IPropertyValue)Activator.CreateInstance(typeof(StructPropertyValue<>).MakeGenericType(valueType), value)!,
            };
        }

        throw new NotImplementedException();
    }
}
