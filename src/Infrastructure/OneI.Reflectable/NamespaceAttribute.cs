namespace OneI.Reflectable;

[AttributeUsage(AttributeTargets.Assembly)]
public class NamespaceAttribute : Attribute
{
    public NamespaceAttribute(string @namespace)
    {
        Namespace = @namespace;
    }

    public string Namespace { get; }
}
