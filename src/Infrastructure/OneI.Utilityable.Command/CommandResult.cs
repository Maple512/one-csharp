namespace OneI;

public readonly struct CommandResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandResult"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="exitedCode">The exited code.</param>
    /// <param name="standardOutput">The standard output.</param>
    /// <param name="standardError">The standard error.</param>
    /// <param name="totalProcessorTime">The total processor time.</param>
    public CommandResult(
        string fileName,
        string arguments,
        int exitedCode,
        string? standardOutput,
        string? standardError,
        TimeSpan totalProcessorTime)
    {
        FileName = fileName;
        Arguments = arguments;
        ExitedCode = exitedCode;
        StandardOutput = standardOutput;
        StandardError = standardError;
        TotalProcessorTime = totalProcessorTime;
    }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string FileName { get; }
    /// <summary>
    /// Gets the arguments.
    /// </summary>
    public string Arguments { get; }
    /// <summary>
    /// Gets the exited code.
    /// </summary>
    public int ExitedCode { get; }
    /// <summary>
    /// Gets the standard output.
    /// </summary>
    public string? StandardOutput { get; }
    /// <summary>
    /// Gets the standard error.
    /// </summary>
    public string? StandardError { get; }
    /// <summary>
    /// Gets the total processor time.
    /// </summary>
    public TimeSpan TotalProcessorTime { get; }

    /// <summary>
    /// 确保执行结果是成功的
    /// </summary>
    /// <param name="suppressOutput">如果失败，是否输出错误信息</param>
    /// <exception cref="CommandFailedException">执行失败时抛出</exception>
    public void EnsureSuccessful(bool suppressOutput = false)
    {
        if(ExitedCode != 0)
        {
            var message = new StringBuilder($"Command failed with exit code {ExitedCode}: {FileName}   {Arguments}");

            if(!suppressOutput)
            {
                if(StandardOutput.NotNullOrEmpty())
                {
                    message.AppendLine($"{Environment.NewLine}Standard Output:{Environment.NewLine}{StandardOutput}");
                }

                if(StandardError.NotNullOrEmpty())
                {
                    message.AppendLine($"{Environment.NewLine}Standard Error:{Environment.NewLine}{StandardError}");
                }
            }

            throw new CommandFailedException(message.ToString());
        }
    }
}
