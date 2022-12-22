namespace OneI.Reflectable;
/// <summary>
/// The namespace attribute.
/// </summary>

[AttributeUsage(AttributeTargets.Assembly)]
public class NamespaceAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NamespaceAttribute"/> class.
    /// </summary>
    /// <param name="namespace">The namespace.</param>
    public NamespaceAttribute(string @namespace)
    {
        Namespace = @namespace;
    }

    /// <summary>
    /// Gets the namespace.
    /// </summary>
    public string Namespace { get; }
}
