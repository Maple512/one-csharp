namespace OneI.Logable.Definitions;

using OneI.Logable.Infrastructure;

public class TypeDefinition
{
    private readonly List<PropertyDefinition> _properties;
    private readonly List<string> _typeArguments;
    private readonly List<string> _contraints;

    public TypeDefinition(
        string name,
        bool isTypeArguments = false,
        TypeKindEnum kind = default)
    {
        Name = name;
        Kind = kind;
        IsTypeArguments = isTypeArguments;

        _typeArguments = new();
        _properties = new();
        _contraints = new();
    }

    /// <summary>
    /// 类型名称（如果是泛型，则不包括类型参数部分）
    /// </summary>
    public string Name { get; }

    public string FullName => ToDisplayString();

    /// <summary>
    /// 类型参数
    /// </summary>
    public IReadOnlyList<string> TypeArguments => _typeArguments;

    public IReadOnlyList<string?> Constraints => _contraints;

    /// <summary>
    /// 是否泛型类型
    /// </summary>
    public bool IsGenericType => _typeArguments.Count != 0;

    /// <summary>
    /// 是否泛型类型参数
    /// </summary>
    public bool IsTypeArguments { get; }

    public TypeKindEnum Kind { get; private set; }

    public IReadOnlyList<PropertyDefinition> Properties => _properties;

    /// <summary>
    /// 添加泛型类型参数
    /// </summary>
    /// <param name="type"></param>
    public void AddTypeArgument(string type)
    {
        _typeArguments.Add(type);
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
        builder.AppendLine($"private static global::OneI.Logable.Templating.Properties.PropertyValue {CodeAssets.CreateMethodName}({FullName} p)");

        builder.AppendLine("{");

        using(var _ = builder.Indent())
        {
            switch(Kind)
            {
                case TypeKindEnum.ValueTuple:
                    break;
                case TypeKindEnum.Nullable:
                    break;
                case TypeKindEnum.Object:
                    break;
                case TypeKindEnum.Array:
                case TypeKindEnum.Enumerable:
                    BuildEnumerable(builder);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToDisplayString()
    {
        var name = IsGenericType
              ? $"{Name}<{string.Join(", ", TypeArguments)}>"
              : $"{Name}";

        return IsTypeArguments ? $"global::{name}" : name;
    }

    public override string ToString() => Name;

    private void Default(IndentedStringBuilder builder)
    {
        builder.AppendLine($"return new global::OneI.Logable.Templating.Properties.ValueTypes.LiteralValue<{FullName}>(p);");
    }

    private void BuildEnumerable(IndentedStringBuilder builder)
    {
        builder.AppendLine($"var array = new global::OneI.Logable.Templating.Properties.ValueTypes.EnumerableValue();");
        builder.AppendLine();

        builder.AppendLine($"foreach(var item in p)");
        builder.AppendLine("{");
        using(var _ = builder.Indent())
        {
            builder.AppendLine($"array.AddPropertyValue(Create(item));");
        }
        builder.AppendLine("}");
        builder.AppendLine();

        builder.AppendLine($"return array;");
    }
}
