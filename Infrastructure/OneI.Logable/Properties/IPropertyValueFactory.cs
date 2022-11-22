namespace OneI.Logable.Properties;

public interface IPropertyValueFactory
{
    PropertyValue Create(object? value, bool deconstruct = false);
}
