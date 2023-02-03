namespace OneI.Logable.Definitions;

public class TypeDef : IEquatable<TypeDef>
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
    }

    public List<string> Names { get; }

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

    /// <summary>
    ///     类型定义的属性
    /// </summary>
    public List<PropertyDef> Properties { get; }

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
            content.Append("global::");
        }

        for(var i = 0; i < Names.Count; i++)
        {
            content.Append(Names[i]);
            if(i < Names.Count - 1)
            {
                content.Append('.');
            }
        }

        if(TypeArguments.Count > 0)
        {
            content.Append('<');

            for(var i = 0; i < TypeArguments.Count; i++)
            {
                content.Append(TypeArguments[i].ToDisplayString());
                if(i < TypeArguments.Count - 1)
                {
                    content.Append(", ");
                }
            }

            content.Append('>');
        }

        if(Kind == TypeDefKind.Array)
        {
            content.Append("[]");
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
    None, Literal, ValueTuple, Nullable, Object, Array, Enumerable, Dictionary,
}
