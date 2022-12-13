namespace OneI.Logable.Definitions;

/// <summary>
/// 描述对象的属性
/// </summary>
public class PropertyDef
{
    public PropertyDef(string name, TypeDef type)
        : this(name, -1, type)
    {
    }

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

    public override string ToString() => Name;
}
