namespace OneI.Logable.Properties;

public interface IProperty
{
    string Name { get; }

    IPropertyValue Value { get; }
}
