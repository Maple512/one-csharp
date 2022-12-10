namespace OneI.Logable;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        //Log.Write()
        Log.Write(LogLevel.Information, "Message", 1);

        Log.Write(LogLevel.Debug, "Message", 3, 4, 5, 65);
    }
}
