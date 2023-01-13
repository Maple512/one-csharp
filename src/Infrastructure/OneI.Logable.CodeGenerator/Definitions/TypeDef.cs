namespace OneI.Logable.Definitions;

/// <summary>
/// 描述对象的类型
/// </summary>
public class TypeDef
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeDef"/> class.
    /// </summary>
    /// <param name="names">The names.</param>
    public TypeDef(params string[] names)
        : this(names.ToList())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeDef"/> class.
    /// </summary>
    /// <param name="names">The names.</param>
    public TypeDef(List<string> names)
    {
        Names = names;

        TypeArguments = new();
        Constraints = new();
        Properties = new();
    }

    /// <summary>
    /// 类型名称
    /// </summary>
    public List<string> Names { get; } = new();

    /// <summary>
    /// 类型参数
    /// </summary>
    public List<TypeDef> TypeArguments { get; }

    /// <summary>
    /// 类型约束
    /// </summary>
    public List<string?> Constraints { get; }

    /// <summary>
    /// 表示泛型类型或泛型方法中的类型参数
    /// </summary>
    public bool IsTypeParameters { get; set; }

    /// <summary>
    /// 是否泛型类型
    /// </summary>
    public bool IsGenericType { get; set; }

    /// <summary>
    /// Gets or sets the kind.
    /// </summary>
    public TypeDefKind Kind { get; set; }

    /// <summary>
    /// 类型定义的属性
    /// </summary>
    public List<PropertyDef> Properties { get; }

    /// <summary>
    /// Equals the.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns>A bool.</returns>
    public override bool Equals(object obj)
    {
        return obj is TypeDef td && td.ToDisplayString() == ToDisplayString();
    }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>An int.</returns>
    public override int GetHashCode()
    {
        return ToDisplayString().GetHashCode();
    }

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        return Names.Join('.');
    }

    /// <summary>
    /// Tos the display string.
    /// </summary>
    /// <returns>A string.</returns>
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
}

/// <summary>
/// 类型的类型（包括：字面量，元组，可空类型，对象，数组，可枚举的集合，字典）
/// </summary>
public enum TypeDefKind : byte
{
    None,

    Literal,

    ValueTuple,

    Nullable,

    Object,

    Array,

    Enumerable,

    Dictionary,
}
