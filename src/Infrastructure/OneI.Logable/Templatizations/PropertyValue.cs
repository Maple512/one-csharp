namespace OneI.Logable.Templatizations;

public static class PropertyValue
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITemplatePropertyValue CreateLiteral<T>(T value, IPropertyValueFormatter<T>? formatter = null)
    {
        return new LiteralValue<T>(value, formatter);
    }
}
