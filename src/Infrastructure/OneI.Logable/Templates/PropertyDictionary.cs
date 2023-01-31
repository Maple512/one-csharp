namespace OneI.Logable.Templates;

using System;
using System.ComponentModel;

/// <summary>
/// 容量：尽量多分配，这个字典不会自动扩容
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public struct PropertyDictionary<TKey, TValue>
    where TKey : IEquatable<TKey>?
{
    const int MaxCapacity = 100;

    private TKey[] _keys;
    private TValue[] _values;
    private int _position;

    [ThreadStatic]
    private static TKey[]? keyBuffer;
    [ThreadStatic]
    private static TValue[]? valuesBuffer;
    [ThreadStatic]
    private static bool _buffered;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PropertyDictionary()
    {
        if (_buffered)
        {
            ThrowNonDisposeException();
        }

        _keys = keyBuffer ??= new TKey[MaxCapacity];
        _values = valuesBuffer ??= new TValue[MaxCapacity];

        _buffered = true;
    }

    public bool IsEmpty
    {
        [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position == 0;
    }

    public int Length
    {
        [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
    }

    [ReadOnly(true)]
    public KeyValuePair<TKey, TValue> this[int index]
    {
        get
        {
            if (index > _position)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
            }

            return new KeyValuePair<TKey, TValue>(_keys[index], _values[index]);
        }
    }

    public ReadOnlySpan<TKey> Keys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _keys.AsSpan(0, _position);
    }

    public ReadOnlySpan<TValue> Values
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _values.AsSpan(0, _position);
    }

    public void AddRange(PropertyDictionary<TKey, TValue> other)
    {
        if (other._position != 0)
        {
            other.Keys.CopyTo(_keys.AsSpan()[_position..]);
            other.Values.CopyTo(_values.AsSpan()[_position..]);
        }
    }

    /// <summary>
    /// 向缓冲区中添加指定的键值对
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">已存在指定的<paramref name="key"/></exception>
    /// <exception cref="ArgumentNullException">给定的<paramref name="key"/>为<see langword="null"/></exception>
    /// <exception cref="ArgumentOutOfRangeException">超出缓冲区长度限制</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(TKey key, TValue value)
    {
        TryInsert(key, value, InsertionBehavior.ThrowOnExisting);
    }

    /// <summary>
    /// 向缓冲区中添加指定的键值对
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns><see langword="true"/>: 成功加入缓冲区，<see langword="false"/>: 未能加入缓冲区</returns>
    /// <exception cref="ArgumentNullException">给定的<paramref name="key"/>为<see langword="null"/></exception>
    /// <exception cref="ArgumentOutOfRangeException">超出缓冲区长度限制</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryAdd(TKey key, TValue value)
    {
        return TryInsert(key, value, InsertionBehavior.None);
    }

    /// <summary>
    /// 向缓冲区中添加指定的键值对，如果指定的<paramref name="key"/>已存在，则更新
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentNullException">给定的<paramref name="key"/>为<see langword="null"/></exception>
    /// <exception cref="ArgumentOutOfRangeException">超出缓冲区长度限制</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddOrUpdate(TKey key, TValue value)
    {
        TryInsert(key, value, InsertionBehavior.OverwriteExisting);
    }

    /// <summary>
    /// 是否包含指定的<paramref name="key"/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(TKey key)
    {
        return Keys.IndexOf(key) != -1;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue? value)
    {
        if (key == null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
        }

        var index = Keys.IndexOf(key);

        if (index != -1)
        {
            value = _values[index];
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// 向缓冲区中添加指定的键值对
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">已存在指定的<paramref name="key"/></exception>
    /// <exception cref="ArgumentNullException">给定的<paramref name="key"/>为<see langword="null"/></exception>
    /// <exception cref="ArgumentOutOfRangeException">超出缓冲区长度限制</exception>
    private bool TryInsert(TKey key, TValue value, InsertionBehavior behavior)
    {
        if (key == null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
        }

        if (_position >= _keys.Length - 1)
        {
            throw new ArgumentOutOfRangeException("To reach the maximum length of the dictionary.");
        }

        var existed = Keys.IndexOf(key) != -1;

        if (existed)
        {
            if (behavior == InsertionBehavior.OverwriteExisting)
            {
                _keys[_position] = key;
                _values[_position] = value;
                return true;
            }

            if (behavior == InsertionBehavior.ThrowOnExisting)
            {
                ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
            }

            return false;
        }

        _keys[_position] = key;
        _values[_position] = value;
        _position++;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (_keys != null)
        {
            _keys = null!;
            _values = null!;
            _position = 0;
            _buffered = false;
        }
    }

    [DoesNotReturn]
    private static void ThrowNonDisposeException()
    {
        throw new InvalidOperationException("Please call the Dispose method after use.");
    }

    internal enum InsertionBehavior : byte
    {
        /// <summary>
        /// The default insertion behavior.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that an existing entry with the same key should be overwritten if encountered.
        /// </summary>
        OverwriteExisting = 1,

        /// <summary>
        /// Specifies that if an existing entry with the same key is encountered, an exception should be thrown.
        /// </summary>
        ThrowOnExisting = 2
    }
}
