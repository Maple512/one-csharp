namespace System.Diagnostics;

using OneI;

/// <summary>
/// The process parameter.
/// </summary>

public sealed class ProcessParameter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessParameter"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    public ProcessParameter(string fileName)
    {
        Check.ThrowIfNullOrWhiteSpace(fileName);

        FileName = fileName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessParameter"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="arguments">The arguments.</param>
    public ProcessParameter(string fileName, string arguments)
        : this(fileName)
    {
        Arguments.AddRange(arguments.Split(' '));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessParameter"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="arguments">The arguments.</param>
    public ProcessParameter(string fileName, params string[] arguments)
        : this(fileName)
    {
        Arguments.AddRange(arguments);
    }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets the arguments.
    /// </summary>
    public List<string> Arguments { get; private set; } = new();

    /// <summary>
    /// Gets or sets the working directory.
    /// </summary>
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Gets the environments.
    /// </summary>
    public Dictionary<string, string?> Environments { get; } = new();

    /// <summary>
    /// Gets the environments to remove.
    /// </summary>
    public List<string> EnvironmentsToRemove { get; } = new();

    /// <summary>
    /// Gets or sets the output receiver.
    /// </summary>
    public Action<bool, string?>? OutputReceiver { get; set; }

    /// <summary>
    /// Gets or sets the output builder.
    /// </summary>
    public StringBuilder? OutputBuilder { get; set; }

    /// <summary>
    /// cmd 命令参数解析：https://www.cnblogs.com/mq0036/p/5244892.html
    /// </summary>
    /// <returns></returns>
    public static ProcessParameter UseCMD(string arguments)
    {
        var parameter = new ProcessParameter("cmd.exe", "/s", "/c");

        parameter.Arguments.AddRange(arguments.Split(' '));

        return parameter;
    }

    /// <summary>
    /// cmd 命令参数解析：https://www.cnblogs.com/mq0036/p/5244892.html
    /// </summary>
    /// <returns></returns>
    public static ProcessParameter UseCMD(params string[] arguments)
    {
        var parameter = new ProcessParameter("cmd.exe", "/s", "/c");

        parameter.Arguments.AddRange(arguments);

        return parameter;
    }

    /// <summary>
    /// Withs the working directory.
    /// </summary>
    /// <param name="workingDirectory">The working directory.</param>
    /// <returns>A ProcessParameter.</returns>
    public ProcessParameter WithWorkingDirectory(string workingDirectory)
    {
        Check.ThrowIfNull(workingDirectory);

        WorkingDirectory = workingDirectory;

        return this;
    }

    /// <summary>
    /// Withs the environment.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>A ProcessParameter.</returns>
    public ProcessParameter WithEnvironment(string name, string value)
    {
        Check.ThrowIfNullOrWhiteSpace(name);

        Environments[name] = value;

        return this;
    }

    /// <summary>
    /// Withs the environment to remove.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A ProcessParameter.</returns>
    public ProcessParameter WithEnvironmentToRemove(string name)
    {
        Check.ThrowIfNullOrWhiteSpace(name);

        EnvironmentsToRemove.Add(name);

        return this;
    }

    /// <summary>
    /// Withs the output.
    /// </summary>
    /// <param name="output">The output.</param>
    /// <returns>A ProcessParameter.</returns>
    public ProcessParameter WithOutput(Action<bool, string?> output)
    {
        Check.ThrowIfNull(output);

        OutputReceiver = output;

        return this;
    }

    /// <summary>
    /// Withs the arguments.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <returns>A ProcessParameter.</returns>
    public ProcessParameter WithArguments(params string[] arguments)
    {
        Arguments.AddRange(arguments);

        return this;
    }
}
