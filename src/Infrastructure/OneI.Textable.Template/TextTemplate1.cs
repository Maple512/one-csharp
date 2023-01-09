namespace OneI.Textable;

using System;
using OneI.Buffers;
using OneI.Textable.Templating.Properties;

public readonly struct TextTemplate1
{
    private readonly ValueBuffer<char> _buffer;
    private readonly Dictionary<string, PropertyValue> _properties;

    public TextTemplate1(ValueBuffer<char> buffer)
    {
        _buffer = buffer;
        _properties = new();
    }

    public static TextTemplate1 Create(scoped in ReadOnlySpan<char> template)
    {
        return new TextTemplate1(new ValueBuffer<char>(template.ToArray()));
    }

    [StackTraceHidden]
    public TextTemplate1 AppendProperty<T>(string name, T value, IFormatter<T>? formatter = null)
    {
        Check.NotNullOrWhiteSpace(name);

        _properties.Add(name, PropertyValue.CreateLiteral(value, formatter));

        return this;
    }
}
