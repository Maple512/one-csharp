namespace OneI.Logable.Definitions;

using System;
using System.Diagnostics;

public class TypeDefinition : IEqualityComparer<TypeDefinition>, IEquatable<TypeDefinition>
{
    private readonly List<PropertyDefinition> _properties;
    private readonly List<string> _typeArguments;
    readonly List<string> _contraints;

    public TypeDefinition(string name)
        : this(name, Enumerable.Empty<string>(), Enumerable.Empty<PropertyDefinition>())
    {
    }

    public TypeDefinition(
        string name,
        IEnumerable<string> typeArguments,
        IEnumerable<PropertyDefinition> properties,
        TypeKindEnum kind = default)
    {
        Name = name;
        Kind = kind;

        _typeArguments = typeArguments.ToList();
        _properties = properties.ToList();

        IsGenericType = _typeArguments.Count != 0;
        FullName = IsGenericType
            ? $"{Name}<{string.Join(", ", TypeArguments)}>"
            : $"{Name}";

        _contraints = new List<string>();
    }

    /// <summary>
    /// 类型名称（如果是泛型，则不包括类型参数部分）
    /// </summary>
    public string Name { get; }

    public string FullName { get; private set; }

    /// <summary>
    /// 类型参数
    /// </summary>
    public IReadOnlyList<string> TypeArguments => _typeArguments;

    public IReadOnlyList<string?> Constraints => _contraints;

    //public bool HasConstraints

    public bool IsGenericType { get; private set; }

    public TypeKindEnum Kind { get; private set; }

    public IReadOnlyList<PropertyDefinition> Properties => _properties;

    /// <summary>
    /// 添加泛型类型参数
    /// </summary>
    /// <param name="type"></param>
    public void AddTypeArgument(string type)
    {
        _typeArguments.Add(type);

        IsGenericType = _typeArguments.Count != 0;

        FullName = IsGenericType
            ? $"{Name}<{string.Join(", ", TypeArguments)}>"
            : $"{Name}";
    }

    /// <summary>
    /// 添加类型的泛型约束
    /// </summary>
    /// <param name="constraint"></param>
    public void AddConstraint(string constraint)
    {
        _contraints.Add(constraint);
    }

    public void AddProperty(PropertyDefinition property)
    {
        _properties.Add(property);
    }

    public void ResetTypeKind(TypeKindEnum kind) => Kind = kind;

    public override string ToString() => $"{FullName}";

    public override int GetHashCode()
    {
        return (FullName, Properties).GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is TypeDefinition td && Equals(td, this);
    }

    public bool Equals(TypeDefinition x, TypeDefinition y)
    {
        return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                && x.Properties.SequenceEqual(y.Properties);
    }

    public bool Equals(TypeDefinition other)
    {
        return Equals(this, other);
    }

    public int GetHashCode(TypeDefinition obj)
    {
        return obj.GetHashCode();
    }
}
