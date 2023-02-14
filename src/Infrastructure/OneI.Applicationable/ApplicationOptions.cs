namespace OneI.Applicationable;

public class ApplicationOptions
{
    public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(30);

    public BackgroundServiceExceptionBehavior BackgroundServiceExceptionBehavior { get; set; } = BackgroundServiceExceptionBehavior.Stop;
}
