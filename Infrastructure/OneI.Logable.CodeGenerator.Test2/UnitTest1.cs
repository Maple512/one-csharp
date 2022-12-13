namespace OneI.Logable;

public class UnitTest1
{
    [Fact]
    public void Test1() => Log.Information("", 1, 2, 3, 4);
}
