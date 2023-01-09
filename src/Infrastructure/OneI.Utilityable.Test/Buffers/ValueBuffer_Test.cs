namespace OneI.Buffers;

using OneI;
using DotNext;
using Shouldly;

using ValueBuffer = ValueBuffer<char>;

public unsafe class ValueBuffer_Test
{
    const string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static IEnumerable<object[]> GetText()
    {
        yield return new object[] { text };
    }

    [Fact]
    public void size_print()
    {
        // size 只能是: 0、1、2、4、8、16、32、64或128
        Unsafe.SizeOf<ValueBuffer>().ShouldBe(16);

        TestTools.PrintLayoutToFile<ValueBuffer>();
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public void end_with(string text)
    {
        var _chars = text.ToCharArray();
        var buffer = text.AsValueBuffer();
        var buffer1 = text[..10].AsValueBuffer();

        buffer.StartsWith(buffer1).ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public void index_of(string text)
    {
        var _chars = text.ToCharArray();

        var index = text.IndexOf('0');

        var buffer = new ValueBuffer(_chars);

        buffer.IndexOf('0').ShouldBe(index);
        buffer.IndexOf(' ').ShouldBe(0);
        buffer.IndexOf(text[0]).ShouldBe(0);
        buffer.IndexOf(text[^1]).ShouldBe(text.Length - 1);

        buffer.LastIndexOf('0').ShouldBe(index);
        buffer.LastIndexOf(' ').ShouldBe(0);
        buffer.LastIndexOf(text[0]).ShouldBe(0);
        buffer.LastIndexOf(text[^1]).ShouldBe(text.Length - 1);

        buffer.Contains('0').ShouldBeTrue();
        buffer.Contains(' ').ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public void sequence_equals(string text)
    {
        var _chars = text.ToCharArray();

        var index = text.IndexOf('0');

        var buffer = new ValueBuffer(_chars);

        buffer.SequenceEqual(new(_chars)).ShouldBeTrue();
        //buffer.SequenceEqual(new(_chars), text.Length).ShouldBeTrue();
        //buffer.SequenceEqual(new(_chars), text.Length + 1).ShouldBeTrue();

        // 和短序列比较
        buffer.SequenceEqual(new(_chars[..6])).ShouldBeFalse();

        // 和短序列在超过的长度中比较
        //buffer.SequenceEqual(new(_chars[..6]), 10).ShouldBeFalse();
    }

    [Fact]
    public void implicit_conversion()
    {
        var array = Randomizer.String(100).ToCharArray();

        ValueBuffer<char> buffer = array;

        buffer.ToArray().ShouldBeEquivalentTo(array);

        var span = new Span<char>(Randomizer.String(100).ToCharArray());

        buffer = span;

        buffer.ToArray().ShouldBeEquivalentTo(span.ToArray());

        buffer.Clear();

        span.ToString().ShouldBe(new string(char.MinValue, span.Length));
    }

    [Fact]
    public void fill_char()
    {
        var buffer = new ValueBuffer(10);

        buffer.Fill('a');

        buffer.ToString().ShouldBeEquivalentTo(new string('a', 10));
    }

    [Fact]
    public void ctor_default()
    {
        var empty = ValueBuffer.Empty;
        empty.IsEmpty.ShouldBeTrue();
        empty.Length.ShouldBe(0);
        empty.ToArray().ShouldBeEmpty();
        empty.ToString().ShouldBeEmpty();

        unsafe
        {
            ref var expected = ref Unsafe.AsRef<char>(null);
            ref var actual = ref MemoryMarshal.GetReference(empty.AsReadOnlySpan());
            Unsafe.AreSame(ref expected, ref actual).ShouldBeTrue();
        }
    }

    [Theory]
    [MemberData(nameof(GetText))]
    [MemberData(nameof(GetText))]
    public void ctor_array(string text)
    {
        var _chars = text.ToCharArray();

        var buffer = new ValueBuffer(_chars);

        buffer.ToArray().ShouldBeEquivalentTo(_chars);

        buffer.ToString().ShouldBeEquivalentTo(new string(_chars));
    }

    [Fact]
    public void ctor_length()
    {
        var buffer = new ValueBuffer(100);

        buffer.ToArray().ShouldBeEquivalentTo(new char[100]);
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public void ctor_array_int(string text)
    {
        var _chars = text.ToCharArray();

        var buffer = new ValueBuffer(_chars, 1, 10);
        buffer.ToArray().ShouldBe(_chars[1..11]);

        var chars = new char[10];

        Should.Throw<ArgumentNullException>(() => new ValueBuffer(null!, 1, 2));

        Should.Throw<ArgumentOutOfRangeException>(() => new ValueBuffer(chars, 5, 10));
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public unsafe void ctor_pointer(string text)
    {
        var _chars = text.ToCharArray();

        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            fixed(char* ptr = &_chars[0])
            {
                _ = new ValueBuffer(ptr, -5);
            }
        });

        fixed(char* ptr = &_chars[0])
        {
            var buffer = new ValueBuffer(ptr, 10);

            buffer.ToArray().ShouldBeEquivalentTo(_chars[..10]);
        }
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public void as_span(string text)
    {
        var _chars = text.ToCharArray();

        var buffer = new ValueBuffer(_chars);

        var rspan = buffer.AsReadOnlySpan();

        rspan.Length.ShouldBe(_chars.Length);

        rspan.ToArray().ShouldBe(_chars);
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public void to_array(string text)
    {
        var _chars = text.ToCharArray();

        var buffer = new ValueBuffer(_chars);

        buffer.ToArray().ShouldBeEquivalentTo(_chars);
        buffer.ToArray().ShouldNotBeSameAs(_chars);
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public void clear(string text)
    {
        var _chars = text.ToCharArray();

        var buffer1 = new ValueBuffer(_chars, 1, 10);
        var buffer2 = new ValueBuffer(_chars, 1, 10);

        buffer1.ToArray().ShouldBeEquivalentTo(buffer2.ToArray());

        buffer1.Clear();

        buffer1.ToArray().ShouldBeEquivalentTo(buffer2.ToArray());
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public void copy_to_other(string text)
    {
        var _chars = text.ToCharArray();

        var buffer = new ValueBuffer(_chars);
        var buffer1 = new ValueBuffer(200);

        buffer.CopyTo(buffer1);

        buffer.ToArray().ShouldBeEquivalentTo(buffer1[.._chars.Length].ToArray());

        var array = stackalloc char[200].ToArray();

        buffer.CopyToArray(array, 0, buffer.Length);

        buffer.ToArray().ShouldBeEquivalentTo(array[.._chars.Length].ToArray());
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public void try_copy_to_other(string text)
    {
        var _chars = text.ToCharArray();

        var buffer = new ValueBuffer(_chars);
        var buffer1 = new ValueBuffer(200);

        buffer.TryCopyTo(buffer1).ShouldBeTrue();
        buffer.ToArray().ShouldBeEquivalentTo(buffer1[.._chars.Length].ToArray());

        var array = stackalloc char[200].ToArray();

        buffer.TryCopyToArray(array, 0, buffer.Length).ShouldBeTrue();

        buffer.ToArray().ShouldBeEquivalentTo(array[.._chars.Length].ToArray());
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public void slice(string text)
    {
        var _chars = text.ToCharArray();

        var buffer = new ValueBuffer(_chars).Slice(_chars.Length);

        buffer.Length.ShouldBe(0);
        Unsafe.AreSame(ref Unsafe.Subtract(ref Unsafe.AsRef(buffer.GetReference()), 1), ref _chars[^1]).ShouldBeTrue();

        var buffer1 = ((ValueBuffer)_chars).Slice(10, 4);

        buffer1.Length.ShouldBe(4);
        Unsafe.AreSame(ref Unsafe.AsRef(buffer1[1]), ref _chars[11]).ShouldBeTrue();
    }

    public static IEnumerable<object[]> IntegerArrays()
    {
        yield return new object[] { new char[0] };
        yield return new object[] { new char[] { (char)42 } };
        yield return new object[] { new char[] { (char)42, (char)43, (char)44, (char)45 } };
    }

    [Theory]
    [MemberData(nameof(IntegerArrays))]
    public static void enumerator(char[] array)
    {
        ValueBuffer span = array;

        var enumerator = span.GetEnumerator();

        var index = 0;
        while(enumerator.MoveNext())
        {
            Unsafe.AreSame(
                ref Unsafe.AsRef(enumerator.Current),
                ref Unsafe.AsRef(span[index++])).ShouldBeTrue();
        }

        enumerator.MoveNext().ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public void reverse(string text)
    {
        var _chars = text.ToCharArray();

        var buffer = new ValueBuffer(_chars);

        var copy = buffer.ToArray();

        Array.Reverse(copy);

        buffer.Reverse();

        buffer.ToArray().ShouldBeEquivalentTo(copy);
    }

    [Theory]
    [MemberData(nameof(GetText))]
    public void type_copy(string text)
    {
        var _chars = text.ToCharArray();

        var buffer = new ValueBuffer(10);

        var str = Randomizer.String(10);

        str.CopyTo(buffer);

        buffer.ToString().ShouldBeEquivalentTo(str);

        buffer.Clear();

        var nb = new ValueBuffer<int>(10);
        var numbers = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        numbers.CopyTo(nb);

        nb.ToArray().ShouldBeEquivalentTo(numbers);
    }

    [Fact]
    public void int_buffer()
    {
        var ints = stackalloc int[100];

        var buffer = new ValueBuffer<int>(ints, 100);

        var array = Enumerable.Range(0, 100)
            .ToArray();

        array.CopyTo(buffer);

        buffer.ToArray().ShouldBeEquivalentTo(array);
    }

    [Fact]
    public void custom_struct_buffer()
    {
        var ints = stackalloc Sturct1[100];

        var buffer = new ValueBuffer<Sturct1>(ints, 100);

        var array = Enumerable.Range(0, 100)
            .Select(x => new Sturct1(100))
            .ToArray();

        array.CopyTo(buffer);

        buffer.ToArray().ShouldBeEquivalentTo(array);
    }

    struct Sturct1
    {
        public int x;

        public Sturct1(int x)
        {
            this.x = x;
        }
    }
}
