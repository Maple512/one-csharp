namespace OneI.Logable.Templates;

using Cysharp.Text;

public interface IPropertyValueFormattable
{
    void Format(ref Utf16ValueStringBuilder writer, PropertyType type);
}
