namespace OneI.Applicationable;

using System;
/// <summary>
/// The application options.
/// </summary>

public class ApplicationOptions
{
    /// <summary>
    /// Gets or sets the arguments.
    /// </summary>
    public string[]? Arguments { get; set; }

    /// <summary>
    /// 停止应用时的超时配置（默认：30s）
    /// </summary>
    public int ShutdownTimeout { get; set; } = ApplicationDefinition.ShutdownTimeout;

    /// <summary>
    /// Gets the shutdown timeout seconds.
    /// </summary>
    public TimeSpan ShutdownTimeoutSeconds => TimeSpan.FromSeconds(ShutdownTimeout);
}
