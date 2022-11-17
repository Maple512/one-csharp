namespace OneI.Logable.Properties;

using System;
using System.IO;

public interface IPropertyValue : IFormattable
{
    void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null);
}

public interface IPropertyValue<TValue> : IPropertyValue
{
    TValue? Value { get; }
}
