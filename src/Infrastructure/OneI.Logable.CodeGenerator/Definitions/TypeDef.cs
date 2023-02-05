namespace OneI.Logable.Definitions;

internal class TypeDef : IEquatable<TypeDef>
{
    public TypeDef(params string[] names)
        : this(names.ToList())
    { }

    public TypeDef(List<string> names)
    {
        Names = names;
        TypeArguments = new();
        Constraints = new();
        Properties = new();
        BaseTypes = new();
        Interfaces = new();
    }

    private string? id;

    public string Id
    {
        get
        {
            id ??= $"{Randomizer.Latter(5)}_{Math.Abs(GetHashCode())}";

            return id;
        }
    }

    public List<string> Names { get; }

    public List<string> BaseTypes { get; }

    public List<string> Interfaces { get; }

    /// <summary>
    ///     类型参数
    /// </summary>
    public List<TypeDef> TypeArguments { get; }

    /// <summary>
    ///     类型约束
    /// </summary>
    public List<string?> Constraints { get; }

    /// <summary>
    ///     表示泛型类型或泛型方法中的类型参数
    /// </summary>
    public bool IsTypeParameters { get; set; }

    /// <summary>
    ///     是否泛型类型
    /// </summary>
    public bool IsGenericType { get; set; }

    /// <summary>
    ///     Gets or sets the kind.
    /// </summary>
    public TypeDefKind Kind { get; set; }

    public bool IsSpanFormattable { get; set; }

    public bool IsFormattable { get; set; }

    public bool IsPropertyValueFormattable { get; set; }

    public bool IsException { get; set; }

    /// <summary>
    ///     类型定义的属性
    /// </summary>
    public List<PropertyDef> Properties { get; }

    public string WrapperMethod
    {
        get
        {
            return $"Create_{WrapperName}";
        }
    }

    public string WrapperName
    {
        get
        {
            return $"Wrapper_{Id}";
        }
    }

    public string FormatterName
    {
        get
        {
            return $"Formatter_{Id}";
        }
    }

    public bool Equals(TypeDef other)
    {
        if(other == null)
        {
            return false;
        }

        return Names.SequenceEqual(other.Names, StringComparer.InvariantCulture)
               && TypeArguments.SequenceEqual(other.TypeArguments)
               && IsTypeParameters == other.IsTypeParameters
               && IsGenericType == other.IsGenericType
               && Kind == other.Kind
               && Properties.SequenceEqual(other.Properties);
    }

    public override bool Equals(object obj) => obj is TypeDef td && Equals(td);

    public override string ToString() => Names.Join('.');

    public string ToDisplayString()
    {
        var content = new StringBuilder();

        if(IsTypeParameters == false)
        {
            _ = content.Append("global::");
        }

        if(Kind is TypeDefKind.Array)
        {
            for(var i = 0; i < TypeArguments.Count; i++)
            {
                _ = content.Append(TypeArguments[i]);
                if(i < TypeArguments.Count - 1)
                {
                    _ = content.Append('.');
                }
            }
        }
        else
        {
            for(var i = 0; i < Names.Count; i++)
            {
                _ = content.Append(Names[i]);
                if(i < Names.Count - 1)
                {
                    _ = content.Append('.');
                }
            }

            if(TypeArguments.Count > 0)
            {
                _ = content.Append('<');

                for(var i = 0; i < TypeArguments.Count; i++)
                {
                    _ = content.Append(TypeArguments[i].ToDisplayString());
                    if(i < TypeArguments.Count - 1)
                    {
                        _ = content.Append(", ");
                    }
                }

                _ = content.Append('>');
            }
        }

        if(Kind == TypeDefKind.Array)
        {
            _ = content.Append("[]");
        }

        return content.ToString();
    }

    public override int GetHashCode()
    {
        var hashCode = -1107522701;
        foreach(var item in Names)
        {
            hashCode = hashCode * -1521134295 + item.GetHashCode();
        }

        foreach(var item in TypeArguments)
        {
            hashCode = hashCode * -1521134295 + item.GetHashCode();
        }

        foreach(var item in Constraints)
        {
            hashCode = hashCode * -1521134295 + item?.GetHashCode() ?? 0;
        }

        hashCode = hashCode * -1521134295 + IsTypeParameters.GetHashCode();
        hashCode = hashCode * -1521134295 + IsGenericType.GetHashCode();
        hashCode = hashCode * -1521134295 + Kind.GetHashCode();
        foreach(var item in Properties)
        {
            hashCode = hashCode * -1521134295 + item.GetHashCode();
        }

        return hashCode;
    }
}

/// <summary>
///     类型的类型（包括：字面量，元组，可空类型，对象，数组，可枚举的集合，字典）
/// </summary>
public enum TypeDefKind : byte
{
    None, Literal, Object, ValueTuple, Tuple, Nullable, Array, Dictionary, EnumerableT, Enumerable,
}
