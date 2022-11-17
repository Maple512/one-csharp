namespace OneI.Logable.Properties;

public interface IPropertyValueFactory
{
    IPropertyValue Create<TValue>(TValue? value);
}
