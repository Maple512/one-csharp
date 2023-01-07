namespace OneI;

using Shouldly;

using ValueBuffer = ValueBuffer<char>;

public unsafe class ValueBuffer_Test
{
    private const string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private static readonly char[] _chars = text.ToCharArray();

    [Fact]
    public void size_print()
    {
        // size 只能是: 0、1、2、4、8、16、32、64或128
        Unsafe.SizeOf<ValueBuffer>().ShouldBe(16);

        TestTools.PrintLayoutToFile<ValueBuffer>();
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

    [Fact]
    public void ctor_array()
    {
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

    [Fact]
    public void ctor_array_int()
    {
        var buffer = new ValueBuffer(_chars, 1, 10);
        buffer.ToArray().ShouldBe(_chars[1..11]);

        var chars = new char[10];

        Should.Throw<ArgumentNullException>(() => new ValueBuffer(null!, 1, 2));

        Should.Throw<ArgumentOutOfRangeException>(() => new ValueBuffer(chars, 5, 10));
    }

    [Fact]
    public unsafe void ctor_pointer()
    {
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

    [Fact]
    public void as_span()
    {
        var buffer = new ValueBuffer(_chars);

        var rspan = buffer.AsReadOnlySpan();

        rspan.Length.ShouldBe(_chars.Length);

        rspan.ToArray().ShouldBe(_chars);
    }

    [Fact]
    public void to_array()
    {
        var buffer = new ValueBuffer(_chars);

        buffer.ToArray().ShouldBeEquivalentTo(_chars);
        buffer.ToArray().ShouldNotBeSameAs(_chars);
    }

    [Fact]
    public void clear()
    {
        var buffer1 = new ValueBuffer(_chars, 1, 10);
        var buffer2 = new ValueBuffer(_chars, 1, 10);

        buffer1.ToArray().ShouldBeEquivalentTo(buffer2.ToArray());

        buffer1.Clear();

        buffer1.ToArray().ShouldBeEquivalentTo(buffer2.ToArray());
    }

    [Fact]
    public void copy_to_other()
    {
        var buffer = new ValueBuffer(_chars);
        var buffer1 = new ValueBuffer(200);

        buffer.CopyTo(buffer1);

        buffer.ToArray().ShouldBeEquivalentTo(buffer1[.._chars.Length].ToArray());

        var array = stackalloc char[200].ToArray();

        buffer.CopyToArray(array, 0, buffer.Length);

        buffer.ToArray().ShouldBeEquivalentTo(array[.._chars.Length].ToArray());
    }

    [Fact]
    public void try_copy_to_other()
    {
        var buffer = new ValueBuffer(_chars);
        var buffer1 = new ValueBuffer(200);

        buffer.TryCopyTo(buffer1).ShouldBeTrue();
        buffer.ToArray().ShouldBeEquivalentTo(buffer1[.._chars.Length].ToArray());

        var array = stackalloc char[200].ToArray();

        buffer.TryCopyToArray(array, 0, buffer.Length).ShouldBeTrue();

        buffer.ToArray().ShouldBeEquivalentTo(array[.._chars.Length].ToArray());
    }

    [Fact]
    public void slice()
    {
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
            Unsafe.AreSame(ref Unsafe.AsRef(enumerator.Current), ref Unsafe.AsRef(span[index++])).ShouldBeTrue();
        }

        enumerator.MoveNext().ShouldBeFalse();
    }

    [Fact]
    public void reverse()
    {
        var buffer = new ValueBuffer(_chars);

        var copy = buffer.ToArray();

        Array.Reverse(copy);

        buffer.Reverse();

        buffer.ToArray().ShouldBeEquivalentTo(copy);
    }

    [Fact]
    public void type_copy()
    {
        var buffer = new ValueBuffer(10);

        var str = Randomizer.String(10);

        str.CopyTo(buffer);

        buffer.ToString().ShouldBeEquivalentTo(str);

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
