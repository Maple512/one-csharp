namespace OneI.Logable.Templates;

using System.ComponentModel;

public struct PropertyDictionary
{
    private const int MaxCapacity = 100;

    private string[] _keys;
    private object?[] _values;

    [ThreadStatic]
    private static string[]? keyBuffer;

    [ThreadStatic]
    private static object?[]? valuesBuffer;

    [ThreadStatic]
    private static bool _buffered;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PropertyDictionary()
    {
        if(_buffered)
        {
            ThrowNonDisposeException();
        }

        _keys = keyBuffer ??= new string[MaxCapacity];
        _values = valuesBuffer ??= new object?[MaxCapacity];

        _buffered = true;
    }

    public bool IsEmpty
    {
        [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Length == 0;
    }

    public int Length
    {
        [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        private set;
    }

    [ReadOnly(true)]
    public KeyValuePair<string, object?> this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Length);

            return new KeyValuePair<string, object?>(_keys[index], _values[index]);
        }
    }

    public ReadOnlySpan<string> Keys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _keys.AsSpan(0, Length);
        }
    }

    public ReadOnlySpan<object?> Values
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _values.AsSpan(0, Length);
        }
    }

    public void AddRange(PropertyDictionary other)
    {
        if(other.Length != 0)
        {
            other.Keys.CopyTo(_keys.AsSpan()[Length..]);
            other.Values.CopyTo(_values.AsSpan()[Length..]);
        }
    }

    /// <summary>
    ///     向缓冲区中添加指定的键值对
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">已存在指定的<paramref name="key" /></exception>
    /// <exception cref="ArgumentNullException">给定的<paramref name="key" />为<see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException">超出缓冲区长度限制</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(string key, object? value)
    {
        _ = TryInsert(key, value, InsertionBehavior.ThrowOnExisting);
    }

    /// <summary>
    ///     向缓冲区中添加指定的键值对
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns><see langword="true" />: 成功加入缓冲区，<see langword="false" />: 未能加入缓冲区</returns>
    /// <exception cref="ArgumentNullException">给定的<paramref name="key" />为<see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException">超出缓冲区长度限制</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryAdd(string key, object? value) => TryInsert(key, value, InsertionBehavior.None);

    /// <summary>
    ///     向缓冲区中添加指定的键值对，如果指定的<paramref name="key" />已存在，则更新
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentNullException">给定的<paramref name="key" />为<see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException">超出缓冲区长度限制</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddOrUpdate(string key, object? value)
        => _ = TryInsert(key, value, InsertionBehavior.OverwriteExisting);

    /// <summary>
    ///     是否包含指定的<paramref name="key" />
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(string key) => Keys.IndexOf(key) != -1;

    public T? GetValue<T>(string key)
    {
        if(key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        var index = Keys.IndexOf(key);

        if(index != -1)
        {
            var obj = _values[index];

            if(obj is T result)
            {
                return result;
            }
        }

        return default;
    }

    public bool TryGetValue<T>(string key, out T? value)
    {
        Check.ThrowIfNull(key);

        var index = Keys.IndexOf(key);

        if(index != -1)
        {
            var obj = _values[index];

            if(obj is T result)
            {
                value = result;
                return true;
            }
        }

        value = default;
        return false;
    }

    public bool TryGetValue(string key, out object? value)
    {
        Check.ThrowIfNull(key);

        var index = Keys.IndexOf(key);

        if(index != -1)
        {
            value = _values[index];
            return true;
        }

        value = null;
        return false;
    }

    /// <summary>
    ///     向缓冲区中添加指定的键值对
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">已存在指定的<paramref name="key" /></exception>
    /// <exception cref="ArgumentNullException">给定的<paramref name="key" />为<see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException">超出缓冲区长度限制</exception>
    private bool TryInsert(string key, object? value, InsertionBehavior behavior)
    {
        Check.ThrowIfNull(key);

        if(Length >= _keys.Length - 1)
        {
            throw new ArgumentOutOfRangeException("To reach the maximum length of the dictionary.");
        }

        var existed = Keys.IndexOf(key) != -1;

        if(existed)
        {
            if(behavior == InsertionBehavior.OverwriteExisting)
            {
                _keys[Length] = key;
                _values[Length] = value;
                return true;
            }

            if(behavior == InsertionBehavior.ThrowOnExisting)
            {
                throw new ArgumentException($"An item with the same key has already been added. Key: {key}");
            }

            return false;
        }

        _keys[Length] = key;
        _values[Length] = value;
        Length++;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if(_keys != null)
        {
            _keys = null!;
            _values = null!;
            Length = 0;
            _buffered = false;
        }
    }

    [DoesNotReturn]
    private static void ThrowNonDisposeException()
        => throw new InvalidOperationException("Please call the Dispose method after use.");

    internal enum InsertionBehavior : byte
    {
        /// <summary>
        ///     The default insertion behavior.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Specifies that an existing entry with the same key should be overwritten if encountered.
        /// </summary>
        OverwriteExisting = 1,

        /// <summary>
        ///     Specifies that if an existing entry with the same key is encountered, an exception should be thrown.
        /// </summary>
        ThrowOnExisting = 2,
    }
}
