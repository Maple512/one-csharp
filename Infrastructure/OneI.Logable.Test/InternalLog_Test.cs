namespace OneI.Logable.Test;

using System.Text.Json;

public class InternalLog_Test
{
    [Fact]
    public void write_log_must_be_initialized_first()
    {
        // Should.Throw<Exception>(() => InternalLog.WriteLine(string.Empty));

        InternalLog.Initialize((message) =>
        {
            Debug.WriteLine(message);
        });

        Should.NotThrow(() => InternalLog.WriteLine(string.Empty));

        var user = new { Id = 1, Name = "Maple512" };

        var dict = new Dictionary<string, object>()
        {
            {"User",user }
        };

        var result = JsonSerializer.Serialize(dict);
    }
}
