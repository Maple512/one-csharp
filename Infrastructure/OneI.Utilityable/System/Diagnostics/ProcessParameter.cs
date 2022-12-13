namespace System.Diagnostics;

using System;
using System.Collections.Generic;
using System.Text;
using OneI;

public sealed class ProcessParameter
{
    public ProcessParameter(string fileName) => FileName = Check.NotNullOrWhiteSpace(fileName);

    public ProcessParameter(string fileName, string arguments)
    {
        FileName = Check.NotNullOrWhiteSpace(fileName);

        Arguments.AddRange(arguments.Split(' '));
    }

    public ProcessParameter(string fileName, params string[] arguments)
    {
        FileName = Check.NotNullOrWhiteSpace(fileName);

        Arguments.AddRange(arguments);
    }

    public string FileName { get; }

    public List<string> Arguments { get; private set; } = new();

    public string? WorkingDirectory { get; set; }

    public Dictionary<string, string?> Environments { get; } = new();

    public List<string> EnvironmentsToRemove { get; } = new();

    public Action<bool, string?>? OutputReceiver { get; set; }

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

    public ProcessParameter WithWorkingDirectory(string workingDirectory)
    {
        WorkingDirectory = Check.NotNull(workingDirectory);

        return this;
    }

    public ProcessParameter WithEnvironment(string name, string value)
    {
        Check.NotNullOrWhiteSpace(name);

        Environments[name] = value;

        return this;
    }

    public ProcessParameter WithEnvironmentToRemove(string name)
    {
        Check.NotNullOrWhiteSpace(name);

        EnvironmentsToRemove.Add(name);

        return this;
    }

    public ProcessParameter WithOutput(Action<bool, string?> output)
    {
        OutputReceiver = Check.NotNull(output);

        return this;
    }

    public ProcessParameter WithArguments(params string[] arguments)
    {
        Arguments.AddRange(arguments);

        return this;
    }
}
