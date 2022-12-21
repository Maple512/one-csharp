namespace OneI;

public readonly struct CommandResult
{
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

    public string FileName { get; }
    public string Arguments { get; }
    public int ExitedCode { get; }
    public string? StandardOutput { get; }
    public string? StandardError { get; }
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
