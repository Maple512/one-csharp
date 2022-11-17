namespace OneT.Common.Test;

using Shouldly;

public unsafe class UnitTest1
{
    [Fact]
    public void Test1()
    {
        sizeof(int).ShouldBe(4);

        sizeof(uint).ShouldBe(4);
    }
}
