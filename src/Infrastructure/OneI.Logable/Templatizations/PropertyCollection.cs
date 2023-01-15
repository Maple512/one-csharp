namespace OneI.Logable.Templatizations;

using DotNext.Collections.Generic;

public class PropertyCollection
{
    private readonly List<ITemplateProperty> _values;
    private readonly Dictionary<int, ITemplatePropertyValue> _indexes;
    private readonly Dictionary<string, ITemplatePropertyValue> _nameds;

    public PropertyCollection(int capacity)
    {
        _values = new(capacity);
        _indexes = new(capacity);
        _nameds = new(capacity, StringComparer.InvariantCulture);
    }

    /// <inheritdoc cref="Dictionary{TKey, TValue}.TryAdd(TKey, TValue)"/>
    public bool Add(int index, ITemplatePropertyValue value)
    {
        return _indexes.TryAdd(index, value);
    }

    /// <summary>
    /// 添加一个指定索引的属性，如果队列中已存在，则更新，防止，则插入
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns><see langword="true"/>: 队列中不存在，<see langword="false"/>：队列中已存在</returns>
    public bool AddOrUpdate(int index, ITemplatePropertyValue value)
    {
        var result = true;

        if(_indexes.ContainsKey(index))
        {
            result = false;
        }

        _indexes[index] = value;

        return result;
    }

    /// <summary>
    /// 将指定名称的属性添加到队列中，如果队列中不存在指定的名称，则添加，如果已存在，则放弃
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Add(string name, ITemplatePropertyValue value)
    {
        return _nameds.TryAdd(name, value);
    }

    /// <summary>
    /// 添加一个指定索引的属性，如果队列中已存在，则更新，反之，则插入
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns><see langword="true"/>: 队列中不存在，<see langword="false"/>：队列中已存在</returns>
    public bool AddOrUpdate(string name, ITemplatePropertyValue value)
    {
        var result = true;

        if(_nameds.ContainsKey(name))
        {
            result = false;
        }

        _nameds[name] = value;

        return result;
    }

    internal void Add(PropertyCollection other)
    {
        _values.AddRange(other._values);

        foreach(var item in other._nameds)
        {
            _nameds[item.Key] = item.Value;
        }

        foreach(var item in other._indexes)
        {
            _indexes[item.Key] = item.Value;
        }
    }

    public int Count => _values.Count;

    public ITemplateProperty this[int index] => _values[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal IReadOnlyList<ITemplateProperty> ToList()
    {
        var result = new ITemplateProperty[_values.Count + _nameds.Count + _indexes.Count];

        _values.CopyTo(result);
        Debugger.Break();
        _nameds.Select(x => new NamedProperty(x.Key, x.Value))
            .ToArray().CopyTo(result, _values.Count);

        _indexes.Select(x => new IndexerProperty(x.Key, x.Value))
            .ToArray().CopyTo(result, _values.Count + _nameds.Count);

        return result;
    }
}
