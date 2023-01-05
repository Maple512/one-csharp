namespace System;

using System.Buffers;
using OneI;

[StructLayout(LayoutKind.Auto)]
public struct ValueBuffer
{
    /// <summary>写下一个字符的位置。</summary>
    private int _pos;

    /// <summary>从缓冲池租用的数组，用于备份<see cref="_buffer"/></summary>
    private char[]? _arrayToReturnToPool;

    /// <summary>
    /// 缓冲区
    /// </summary>
    private char[] _buffer;

    /// <summary>
    /// 创建一个字符串缓存器
    /// </summary>
    public ValueBuffer() : this(GlobalConstants.ArrayPoolMinimumLength) { }

    /// <summary>
    /// 创建一个字符串缓存器
    /// </summary>
    /// <param name="capacity">缓冲区容量。请选择合适的容量，以尽量减少扩容次数</param>
    public ValueBuffer(int capacity)
    {
        _buffer = _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(capacity);
        _pos = 0;
    }

    public ValueBuffer(scoped Span<char> buffer)
    {
        _arrayToReturnToPool = null;
        _buffer = buffer.ToArray();
        _pos = 0;
    }

    public override string ToString() => Text.ToString();

    /// <summary>
    /// 获取已缓存的字符串，并清理缓冲区
    /// </summary>
    /// <remarks>
    /// 该方法只能调用一次
    /// </remarks>
    public string ToStringAndClear()
    {
        var result = Text.ToString();

        Clear();

        return result;
    }

    /// <summary>清理缓存，并返还租用的缓冲池</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        var toReturn = _arrayToReturnToPool;

        this = default;

        if(toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn, true);
        }
    }

    private readonly ReadOnlySpan<char> Text => _buffer.AsSpan()[.._pos];

    private Span<char> Span => _buffer.AsSpan();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(scoped in ReadOnlySpan<char> value)
    {
        if(value.IsEmpty)
        {
            return;
        }

        var pos = _pos;
        if(value.Length == 1
            && (uint)pos < (uint)_buffer.Length)
        {
            _buffer[pos] = value[0];
            _pos = pos + 1;
            return;
        }

        GrowThenCopyString(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine()
    {
        Append(Environment.NewLine);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine(in string value)
    {
        Append(value);

        Append(Environment.NewLine);
    }

    /// <summary>
    /// 当缓冲区空间不足时，自动扩容
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopyString(scoped in ReadOnlySpan<char> value)
    {
        var pos = _pos;
        if(pos > _buffer.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(Span[_pos..]);

        _pos += value.Length;
    }

    /// <summary>
    /// 扩容
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)] // keep consumers as streamlined as possible
    private void Grow(int additionalChars)
    {
        // 当剩余空间(_buffer.Length - _pos)不足时，调用此方法
        // 至少需要扩张(_pos + additionalChars)长度
        Debug.Assert(additionalChars > _buffer.Length - _pos);

        GrowCore((uint)_pos + (uint)additionalChars);
    }

    /// <summary>Grow the size of <see cref="_buffer"/> to at least the specified <paramref name="requiredMinCapacity"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // but reuse this grow logic directly in both of the above grow routines
    private void GrowCore(uint requiredMinCapacity)
    {
        // 我们希望我们实际需要的空间最大化，容量翻倍（不超过最大允许长度）。
        // 我们还希望避免使用小数组，以减少我们需要增长的次数，并且由于我们使用的是无符号整数，
        // 如果有人试图（例如）将一个巨大字符串附加到一个巨大的字符串上，则在技术上可能会溢出，
        // 因此我们也会钳制int.MaxValue。即使在这种情况下数组创建失败，我们也可能会在ToStringAndClear中失败。

        var newCapacity = Math.Max(requiredMinCapacity, Math.Min((uint)_buffer.Length * 2, GlobalConstants.StringMaxLength));
        var arraySize = (int)Math.Clamp(newCapacity, GlobalConstants.ArrayPoolMinimumLength, int.MaxValue);
        var newArray = ArrayPool<char>.Shared.Rent(arraySize);

        Text.CopyTo(newArray);

        var toReturn = _arrayToReturnToPool;

        _buffer = _arrayToReturnToPool = newArray;

        if(toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn, true);
        }
    }
}
