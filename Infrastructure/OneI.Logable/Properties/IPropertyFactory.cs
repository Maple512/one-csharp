namespace OneI.Logable.Properties;

public interface IPropertyFactory
{
    Property Create(string name, object? value, bool deconstruct = false);
}
