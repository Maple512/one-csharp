namespace OneI.Logable.Templates;

public readonly struct PropertyCollection
{
    private readonly Dictionary<string, ITemplatePropertyValue> _properties;

    internal PropertyCollection(PropertyCollection other)
    {
        _properties = other._properties;
    }

    public PropertyCollection() : this(0) { }

    public PropertyCollection(int capacity)
    {
        _properties = new(capacity, StringComparer.InvariantCulture);
    }

    internal IReadOnlyDictionary<string, ITemplatePropertyValue> NamedProperties
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _properties;
    }

    /// <summary>
    /// 将指定名称的属性添加到队列中，如果队列中不存在指定的名称，则添加，如果已存在，则放弃
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Add(string name, ITemplatePropertyValue value)
    {
        return _properties.TryAdd(name, value);
    }

    /// <summary>
    /// 添加一个指定索引的属性，如果队列中已存在，则更新，反之，则插入
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void AddOrUpdate(string name, ITemplatePropertyValue value)
    {
        _properties[name] = value;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _properties.Count;
    }
}
