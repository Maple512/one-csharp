namespace OneI.Logable.File.Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var logger = TestHelper.CreateLogger();

        logger.Debug("", 2, 3, 53,4,5,56,6,7,8, "dafasdfasd");
    }
}
