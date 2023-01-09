namespace OneI.Buffers;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using DotNext.Runtime;

/// <summary>
/// 字符缓冲区，参考<see cref="Span{T}"/>
/// </summary>
[StackTraceHidden]
[StructLayout(LayoutKind.Auto)]
public readonly unsafe struct ValueBuffer<T> : IEquatable<ValueBuffer<T>>
    where T : unmanaged
{
    private readonly uint _length;
    private readonly T* _reference;

    public static readonly ValueBuffer<T> Empty = default;

    /// <summary>
    /// 在指定的数组上创建<see cref="ValueBuffer{T}"/>
    /// </summary>
    /// <param name="array"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueBuffer(T[]? array)
        : this(array, 0, array?.Length ?? 0)
    {
    }

    /// <summary>
    /// 创建指定长度的<see cref="ValueBuffer{T}"/>
    /// </summary>
    /// <param name="length"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueBuffer(int length)
        : this(new T[length], start: 0, length)
    {
    }

    /// <summary>
    /// 从指定索引开始，创建包含数组指定长度的<see cref="ValueBuffer{T}"/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueBuffer(T[]? array, int start, int length)
    {
        if(array == null || array.Length == 0)
        {
            if(start != 0 || length != 0)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }

            this = default;
            return;
        }

#if TARGET_64BIT
        if((ulong)(uint)start + (ulong)(uint)length > (ulong)(uint)array.Length)
        {
            ThrowArgumentOutofRangeException();
        }
#else
        if((uint)start > (uint)array.Length || (uint)length > (uint)(array.Length - start))
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }
#endif

        _length = (uint)length;

        fixed(T* ptr = array)
        {
            _reference = ptr + start;
        }
    }

    /// <summary>
    /// 从指定的内存地址开始，创建指定长度的<see cref="ValueBuffer{T}"/>
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="length"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueBuffer(void* reference, int length)
    {
        if(length < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        _reference = (T*)reference;
        _length = (uint)length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueBuffer(ref T reference, int length)
    {
        if(length < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        _reference = (T*)Unsafe.AsPointer(ref reference);
        _length = (uint)length;
    }

    public int Length => (int)_length;

    public bool IsEmpty => _length == 0;

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var uindex = (uint)index;

            if(uindex >= _length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }

            return ref _reference[uindex];
        }
    }

    /// <summary>
    /// 是否相同的地址
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool AreSame(ValueBuffer<T> other)
    {
        return _reference == other._reference;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public T* GetPointer() => _reference;

    public void Fill(T value)
    {
        if(_length > 0)
        {
            var index = 0;
            do
            {
                *(_reference + index++) = value;
            } while(index < _length);
        }
    }

    /// <summary>
    /// 返回对第0个元素的引用
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ref T GetReference() => ref *_reference;

    public ReadOnlySpan<T> AsReadOnlySpan()
    {
        return new ReadOnlySpan<T>(_reference, Length);
    }

    public Span<T> AsSpan()
    {
        return new Span<T>(_reference, Length);
    }

    /// <summary>
    /// 将此实例中的所有字符复制到新的数组中
    /// </summary>
    /// <returns></returns>
    public T[] ToArray()
    {
        if(_length == 0)
        {
            return Array.Empty<T>();
        }

        var array = new T[_length];

        CopyToArray(array, 0, Length);

        return array;
    }

    /// <summary>
    /// 清空该实例的所有内容
    /// </summary>
    public void Clear()
    {
        if(_length > 0)
        {
            Unsafe.InitBlockUnaligned(_reference, 0, BitLength);
        }
    }

    /// <summary>
    /// 将此实例中的所有字符复制到目标<paramref name="destination"/>中
    /// </summary>
    /// <param name="destination"></param>
    public void CopyTo(ValueBuffer<T> destination)
    {
        if(destination.IsEmpty || IsEmpty)
        {
            return;
        }

        Buffer.MemoryCopy(_reference, destination._reference, destination.BitLength, BitLength);
    }

    /// <summary>
    /// 尝试将此实例中的所有字符复制到目标<paramref name="destination"/>中
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    public bool TryCopyTo(ValueBuffer<T> destination)
    {
        if(destination.IsEmpty || IsEmpty)
        {
            return false;
        }

        if(_length <= destination._length)
        {
            Buffer.MemoryCopy(_reference, destination._reference, destination.BitLength, BitLength);

            return true;
        }

        return false;
    }

    /// <summary>
    /// 将此实例中的字符复制到指定的数组中
    /// </summary>
    /// <param name="array"></param>
    /// <param name="start">此实例内子字符串的起始位置</param>
    /// <param name="length">此实例内子字符串的长度</param>
    public void CopyToArray(T[] array, int start, int length)
    {
        if(array is null or { Length: 0 }
            || length == 0)
        {
            return;
        }

        fixed(T* ptr = array)
        {
            Buffer.MemoryCopy(_reference + start, ptr, array.Length * Unsafe.SizeOf<T>(), BitLength);
        }
    }

    /// <summary>
    /// 尝试将此实例中的字符复制到指定的数组中
    /// </summary>
    /// <param name="array"></param>
    /// <param name="start">此实例内子字符串的起始位置</param>
    /// <param name="length">此实例内子字符串的长度</param>
    /// <returns></returns>
    public bool TryCopyToArray(T[] array, int start, int length)
    {
        if((uint)length <= _length)
        {
            fixed(T* ptr = array)
            {
                Buffer.MemoryCopy(_reference + start, ptr, array.Length * Unsafe.SizeOf<T>(), BitLength);
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// 切片
    /// </summary>
    /// <param name="start">开始此切片的索引</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ValueBuffer<T> Slice(int start)
    {
        var ustart = (uint)start;
        if(ustart > _length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        var length = _length - ustart;

        return Slice(start, (int)length);
    }

    /// <summary>
    /// 切片
    /// </summary>
    /// <param name="start">开始此切片的索引</param>
    /// <param name="length">此切片的长度</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ValueBuffer<T> Slice(int start, int length)
    {
        //https://github.com/dotnet/runtime/blob/6009a1064ccfc2bd9aeb96c9247b60cf6352198d/src/libraries/System.Private.CoreLib/src/System/Span.cs#L415
#if TARGET_64BIT
        if(((ulong)(uint)start + (ulong)(uint)length) > (ulong)_length)
        {
            ThrowArgumentOutofRangeException(nameof(length));
        }
#else
        if((uint)start + (uint)length > _length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }
#endif

        return new ValueBuffer<T>(_reference + start, length);
    }


    public static ValueBuffer<char> Create(in string text)
    {
        fixed(char* reference = text)
        {
            return new ValueBuffer<char>(reference, text.Length);
        }
    }

    public override string ToString()
    {
        if(typeof(T) == typeof(char))
        {
            if(IsEmpty)
            {
                return string.Empty;
            }

            return new string((char*)_reference, 0, (int)_length);
        }

        return $"OneI.ValueBuffer<{typeof(T).Name}>[{_length}]";
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ValueBuffer<T> buffer
            && Equals(buffer);
    }

    public bool Equals(ValueBuffer<T> other)
    {
        return _reference == other._reference
            && _length == other._length;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Intrinsics.PointerHashCode(_reference), _length);
    }

    public static implicit operator ValueBuffer<T>(T[] array)
    {
        if(array is null or { Length: 0 })
        {
            return Empty;
        }

        return new ValueBuffer<T>(array);
    }

    public static implicit operator ValueBuffer<T>(ArraySegment<T> segment)
    {
        if(segment.Array is null
            || segment.Count is 0)
        {
            return Empty;
        }

        return new ValueBuffer<T>(segment.Array[segment.Offset..segment.Count]);
    }

    public static implicit operator ValueBuffer<T>(Span<T> span)
    {
        if(span.IsEmpty)
        {
            return Empty;
        }

        return new ValueBuffer<T>(Unsafe.AsPointer<T>(ref span.GetPinnableReference()), span.Length);
    }

    public static implicit operator ReadOnlyValueBuffer<T>(ValueBuffer<T> span)
    {
        if(span.IsEmpty)
        {
            return ReadOnlyValueBuffer<T>.Empty;
        }

        return new ReadOnlyValueBuffer<T>(span._reference, span.Length);
    }

    public static bool operator ==(ValueBuffer<T> left, ValueBuffer<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ValueBuffer<T> left, ValueBuffer<T> right)
    {
        return !(left == right);
    }

    private uint BitLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length * (uint)sizeof(T);
    }

    public Enumerator GetEnumerator() => new(this);

    public struct Enumerator
    {
        private readonly ValueBuffer<T> _buffer;
        private int _index;

        internal Enumerator(ValueBuffer<T> buffer)
        {
            _buffer = buffer;
            _index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var index = _index + 1;
            if(index < _buffer.Length)
            {
                _index = index;

                return true;
            }

            return false;
        }

        public ref T Current => ref _buffer[_index];
    }
}
