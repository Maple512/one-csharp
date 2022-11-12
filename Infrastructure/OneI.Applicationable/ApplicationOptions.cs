namespace OneI.Applicationable;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

public class ApplicationOptions
{
    public string[]? Arguments { get; set; }

    /// <summary>
    /// 停止应用时的超时配置（默认：30s）
    /// </summary>
    public int ShutdownTimeout { get; set; } = ApplicationDefinition.ShutdownTimeout;

    public TimeSpan ShutdownTimeoutSeconds => TimeSpan.FromSeconds(ShutdownTimeout);
}
