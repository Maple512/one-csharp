namespace OneI;

public class SizeOf_Test
{
    [Fact]
    public unsafe void size_of()
    {
        sizeof(byte).ShouldBe(1);
        sizeof(sbyte).ShouldBe(1);

        sizeof(short).ShouldBe(2);
        sizeof(ushort).ShouldBe(2);

        sizeof(int).ShouldBe(4);
        sizeof(uint).ShouldBe(4);
        sizeof(float).ShouldBe(4);

        sizeof(long).ShouldBe(8);
        sizeof(nint).ShouldBe(8);
        sizeof(nuint).ShouldBe(8);
        sizeof(ulong).ShouldBe(8);
        sizeof(double).ShouldBe(8);

        sizeof(decimal).ShouldBe(16);

        sizeof(byte*).ShouldBe(8);
        sizeof(sbyte*).ShouldBe(8);
        sizeof(short*).ShouldBe(8);
        sizeof(ushort*).ShouldBe(8);
        sizeof(int*).ShouldBe(8);
        sizeof(uint*).ShouldBe(8);
        sizeof(float*).ShouldBe(8);
        sizeof(long*).ShouldBe(8);
        sizeof(nint*).ShouldBe(8); // IntPtr
        sizeof(nuint*).ShouldBe(8); // UIntPtr
        sizeof(ulong*).ShouldBe(8);
        sizeof(double*).ShouldBe(8);
        sizeof(decimal*).ShouldBe(8);
    }

    [Fact]
    public unsafe void get_full_path()
    {
        // SafeFileHandle
    }
}
