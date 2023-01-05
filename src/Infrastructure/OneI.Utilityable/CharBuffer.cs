namespace OneI;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// 字符缓冲区，参考<see cref="Span{T}"/>
/// </summary>
public readonly unsafe struct CharBuffer : IEquatable<CharBuffer>
{
    private readonly int _length;
    private readonly char* _reference;
    private const int bitSize = sizeof(char);

    public CharBuffer() : this(GlobalConstants.ArrayPoolMinimumLength) { }

    public CharBuffer(char[] array)
        : this(array, array.Length)
    {
    }

    public CharBuffer(int capacity)
        : this(new char[capacity], capacity)
    {

    }

    public CharBuffer(char[] array, int length)
    {
        if(array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        _length = array.Length;

        fixed(char* prt = &array[0])
        {
            _reference = prt;
        }
    }

    public CharBuffer(char* reference, int length)
    {
        _reference = reference;
        _length = length;
    }

    public CharBuffer(ref char b)
    {
        fixed(char* ptr = &b)
        {
            _reference = ptr;
        }

        _length = 1;
    }

    public int Length => _length;

    public bool IsEmpty => _length == 0;

    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if((uint)index >= (uint)_length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return ref _reference[index];
        }
    }

    public char[] ToArray()
    {
        var array = new char[_length];

        CopyTo(array, 0, _length);

        return array;
    }

    public void Clear()
    {
        Unsafe.InitBlockUnaligned(_reference, 0, (uint)BitLength);
    }

    public void CopyTo(CharBuffer destination)
    {
        var length = (uint)destination.Length * bitSize;

        Buffer.MemoryCopy(_reference, destination._reference, destination.BitLength, BitLength);
    }

    public bool TryCopyTo(CharBuffer destination)
    {
        if((uint)_length <= (uint)destination.Length)
        {
            Buffer.MemoryCopy(_reference, destination._reference, destination.BitLength, BitLength);

            return true;
        }

        return false;
    }

    public void CopyTo(char[] bytes, int start, int length)
    {
        fixed(char* ptr = bytes)
        {
            Buffer.MemoryCopy(_reference + start, ptr, bytes.Length * bitSize, BitLength);
        }
    }

    public bool TryCopyTo(char[] bytes, int start, int length)
    {
        if((uint)length <= (uint)_length)
        {
            fixed(char* ptr = bytes)
            {
                Buffer.MemoryCopy(_reference + start, ptr, bytes.Length * bitSize, BitLength);
            }

            return true;
        }

        return false;
    }

    public char[] ToCharArray()
    {
        var chars = new char[_length];

        for(int i = 0; i < _length; i++)
        {
            chars[i] = Unsafe.As<char, char>(ref *(_reference + i));
        }

        return chars;
    }

    /// <summary>
    /// 切片
    /// </summary>
    /// <param name="start">开始此切片的索引</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public CharBuffer Slice(int start)
    {
        if((uint)start > (uint)_length)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }

        var length = _length - start;

        return new CharBuffer((char*)Unsafe.Add<char>(_reference, start), length);
    }

    /// <summary>
    /// 切片
    /// </summary>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public CharBuffer Slice(int start, int length)
    {
#if TARGET_64BIT
            if ((ulong)(uint)start + (ulong)(uint)length > (ulong)(uint)_length)
            {
                throw new ArgumentOutOfRangeException();
            }
#else
        if((uint)start > (uint)_length || (uint)length > (uint)(_length - start))
        {
            throw new ArgumentOutOfRangeException();
        }
#endif

        return new CharBuffer((char*)Unsafe.Add<char>(_reference, start), length);
    }

    public static CharBuffer Create(in string text)
    {
        fixed(char* reference = text)
        {
            return new CharBuffer(reference, text.Length);
        }
    }

    public static CharBuffer Create(int capacity)
    {
        var chars = new char[capacity];

        fixed(char* ptr = chars)
        {
            return new(ptr, capacity);
        }
    }

    public override string ToString()
    {
        return new string(_reference);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is CharBuffer buffer
            && Equals(buffer);
    }

    public bool Equals(CharBuffer other)
    {
        return _reference == other._reference
            && _length == other._length;
    }

    [Obsolete("GetHashCode() on Span will always throw an exception.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS0809 // 过时成员重写未过时成员
    public override int GetHashCode() => throw new NotSupportedException();
#pragma warning restore CS0809 // 过时成员重写未过时成员

    public static bool operator ==(CharBuffer left, CharBuffer right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CharBuffer left, CharBuffer right)
    {
        return !(left == right);
    }

    private int BitLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length * bitSize;
    }
}
