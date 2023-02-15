namespace OneI.Applicationable;

public sealed class CancellactionToken_Test
{
    [Fact]
    public void cancel_token()
    {
        var source = new CancellationTokenSource();

        var registration = source.Token.Register(() =>
           {
               Debugger.Break();
           });

        // 释放后，不会触发断点
        registration.Dispose();

        source.Cancel();
    }
}
