namespace OneI.Logable.Templates;

public interface IPropertyValueFormattable
{
    bool TryFormat(Span<char> destination, PropertyType type, out int charsWritten);
}
