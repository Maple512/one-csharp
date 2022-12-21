namespace OneI.Diagnostics;

public class DebugWatch_Test
{
    [Fact]
    public async void stop_watch()
    {
        var output = new StringWriter();

        var reciver = (int count, TimeSpan time) => output.WriteLine($"{count}: {time.TotalMilliseconds:N3} ms");

        DebugWatch.Start(reciver);

        await Task.Delay(10);

        DebugWatch.Stop();

        await Task.Delay(10);

        DebugWatch.Mark();

        await Task.Delay(10);

        DebugWatch.EndAndReport();

        var result = output.ToString();
    }
}
