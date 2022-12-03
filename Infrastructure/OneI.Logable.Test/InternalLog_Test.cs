namespace OneI.Logable.Test;

public class InternalLog_Test
{
    [Fact]
    public void write_log_must_be_initialized_first()
    {
        Should.Throw<Exception>(() => InternalLog.WriteLine(string.Empty));

        InternalLog.Initialize((message) =>
        {
            Debug.WriteLine(message);
        });

        Should.NotThrow(() => InternalLog.WriteLine(string.Empty));
    }
}
