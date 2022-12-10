namespace OneI.Logable.Definitions;

using OneI.Logable.Infrastructure;

public class TypeDefinition
{
    private readonly List<PropertyDefinition> _properties;
    private readonly List<string> _typeArguments;
    private readonly List<string> _contraints;

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
    public void AddConstraint(string constraint) => _contraints.Add(constraint);

    public void AddProperty(PropertyDefinition property) => _properties.Add(property);

    public void ResetTypeKind(TypeKindEnum kind) => Kind = kind;

    public void AppendTo(IndentedStringBuilder builder)
    {
        builder.AppendLine($"private static global::OneI.Logable.Templating.Properties.PropertyValue {CodeAssets.CreatePropertyValueMethodName}({FullName} p)");

        builder.AppendLine("{");

        using (var _ = builder.Indent())
        {
            switch(Kind)
            {
                case TypeKindEnum.ValueTuple:
                    break;
                case TypeKindEnum.Nullable:
                    break;
                case TypeKindEnum.Object:
                    break;
                case TypeKindEnum.Enumerable:
                    break;
                case TypeKindEnum.Dictionary:
                    break;
                default:
                    Default(builder);
                    break;
            }
        }

        builder.AppendLine("}");
    }

    public override string ToString() => $"{FullName}";

    public override int GetHashCode() => (FullName, Properties).GetHashCode();

    private void Default(IndentedStringBuilder builder)
    {
        builder.AppendLine($"return new global::OneI.Logable.Templating.Properties.ValueTypes.LiteralValue<{FullName}>(p);");
    }
}
