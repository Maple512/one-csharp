namespace OneI.Logable.Properties;

/// <summary>
/// TODO: 属性解构器，待优化
/// </summary>
public interface IPropertyDeconstructor
{
    bool IsSupported(object value);

    PropertyValue Deconstruct(object value);
}
