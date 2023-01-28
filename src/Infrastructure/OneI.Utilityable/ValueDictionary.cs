namespace OneI;

using System.Collections;

[StructLayout(LayoutKind.Auto)]
public struct ValueDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull, IEquatable<TKey>?
{
    private int _position;
    private TKey[] _keys;
    private TValue[] _values;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueDictionary() : this(0) { }

    public ValueDictionary(int capacity)
    {
        if(capacity == 0)
        {
            _keys = Array.Empty<TKey>();
            _values = Array.Empty<TValue>();

            return;
        }

        _keys = new TKey[capacity];
        _values = new TValue[capacity];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueDictionary(ValueDictionary<TKey, TValue> other)
    : this(other, 0) { }

    public ValueDictionary(ValueDictionary<TKey, TValue> other, int growth)
    {
        if(growth < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(growth));
        }

        _position = other._position + growth;

        _keys = new TKey[_position];
        _values = new TValue[_position];

        other._keys.CopyTo(_keys.AsSpan());
        other._values.CopyTo(_values.AsSpan());
    }

    public readonly KeyValuePair<TKey, TValue> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if(index < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(index));
            }

            if(index > _position)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
            }

            var key = _keys[index];
            var value = _values[index];

            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
    }

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _keys.Length;
    }

    public ValueDictionary(KeyValuePair<TKey, TValue>[] collections)
    {
        _position = collections.Length;
        _keys = new TKey[collections.Length];
        _values = new TValue[collections.Length];

        for(var i = 0; i < collections.Length; i++)
        {
            _keys[i] = collections[i].Key;
            _values[i] = collections[i].Value;
        }
    }

    public IReadOnlyDictionary<TKey, TValue> ToDictionary()
    {
        var dic = new Dictionary<TKey, TValue>(_position);

        for(var i = 0; i < _position; i++)
        {
            dic[_keys[i]] = _values[i];
        }

        return dic;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(TKey key, TValue value)
    {
        TryInsertCore(key, value, InsertionBehavior.ThrowOnExisting);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AddOrUpdate(TKey key, TValue value)
    {
        return TryInsertCore(key, value, InsertionBehavior.OverwriteExisting);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryAdd(TKey key, TValue value)
    {
        return TryInsertCore(key, value, InsertionBehavior.None);
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        var index = _keys.AsSpan().IndexOf(key);

        if(index == -1)
        {
            value = default;

            return false;
        }

        value = _values[index];

        return true;
    }

    public void Clear()
    {
        var position = _position;
        if(position > 0)
        {
            _position = 0;
            _keys.AsSpan().Clear();
            _values.AsSpan().Clear();
        }
    }

    public bool ContainsKey(TKey key)
    {
        if(key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return _keys.AsSpan().IndexOf(key) != -1;
    }

    private bool TryInsertCore(TKey key, TValue value, InsertionBehavior behavior)
    {
        if(key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        var index = _keys.AsSpan().IndexOf(key);

        if(index != -1)
        {
            if(behavior == InsertionBehavior.OverwriteExisting)
            {
                _keys[index] = key;
                _values[index] = value;

                return true;
            }

            if(behavior == InsertionBehavior.ThrowOnExisting)
            {
                throw new ArgumentException($"An item with the same key has already been added. Key: {key}");
            }

            return false;
        }

        var position = _position;
        if(position < _keys.Length)
        {
            _keys[position] = key;
            _values[position] = value;
            _position++;
        }
        else
        {
            GrowAndAppend(key, value);
        }

        return true;
    }

    public void AddRange(KeyValuePair<TKey, TValue>[] collections)
    {
        if(collections is not { Length: > 0 })
        {
            return;
        }

        foreach(var item in collections)
        {
            Add(item.Key, item.Value);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(TKey key, TValue value)
    {
        Grow(1);

        Add(key, value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int growth)
    {
        var position = _position;

        var newCapacity = Math.Max(position + growth, Math.Min(_keys.Length * 2, SharedConstants.ArrayMaxLength));

        var keys = new TKey[newCapacity];
        var values = new TValue[newCapacity];

        _keys[.._position].CopyTo(keys.AsSpan());
        _values[.._position].CopyTo(values.AsSpan());

        _keys = keys;
        _values = values;
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return new Enumerator(this);
    }

    public IEnumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly ValueDictionary<TKey, TValue> _dictionary;
        private int _index;

        public Enumerator(ValueDictionary<TKey, TValue> dictionary) : this()
        {
            _dictionary = dictionary;
            _index = -1;
        }

        public KeyValuePair<TKey, TValue> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _dictionary[_index];
        }

        object IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _dictionary[_index];
        }

        public void Dispose() { }

        public bool MoveNext()
        {
            var index = _index + 1;
            if(index < _dictionary.Length)
            {
                _index = index;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _index = -1;
        }
    }

    private enum InsertionBehavior : sbyte
    {
        None = 0,
        OverwriteExisting = 1,
        ThrowOnExisting = 2
    }
}
