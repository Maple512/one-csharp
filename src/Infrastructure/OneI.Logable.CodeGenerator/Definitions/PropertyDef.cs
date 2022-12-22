namespace OneI.Logable.Definitions;

/// <summary>
/// 描述对象的属性
/// </summary>
public class PropertyDef
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyDef"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    public PropertyDef(string name, TypeDef type)
        : this(name, -1, type)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyDef"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="index">The index.</param>
    /// <param name="type">The type.</param>
    public PropertyDef(string name, int index, TypeDef type)
    {
        Name = name;
        Index = index;
        Type = type;
    }

    /// <summary>
    /// 属性名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 索引
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// 类型
    /// </summary>
    public TypeDef Type { get; }

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        return Name;
    }
}
