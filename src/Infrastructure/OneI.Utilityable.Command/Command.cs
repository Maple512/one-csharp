namespace OneI;

using System.ComponentModel;
using System.Diagnostics;
using OneI.Diagnostics;

/// <summary>
/// source: https://github.com/dotnet/runtime/blob/main/src/installer/tests/TestUtils/Command.cs
/// </summary>
public class Command
{
    private StringWriter? _stdOutCapture;
    private StringWriter? _stdErrCapture;
    private StopwatchValue _stopwatch;

    private bool _running = false;

    /// <summary>
    /// Gets the process.
    /// </summary>
    public Process Process { get; }

    // Priority order of runnable suffixes to look for and run
    private static readonly string[] RunnableSuffixes = OperatingSystem.IsWindows()
                                                     ? new string[] { ".exe", ".cmd", ".bat" }
                                                     : new string[] { string.Empty };

    /// <summary>
    /// Prevents a default instance of the <see cref="Command"/> class from being created.
    /// </summary>
    /// <param name="executable">The executable.</param>
    /// <param name="args">The args.</param>
    private Command(string executable, string args)
    {
        // Set the things we need
        var psi = new ProcessStartInfo()
        {
            FileName = executable,
            Arguments = args,
            CreateNoWindow = true,
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        Process = new Process()
        {
            StartInfo = psi
        };
    }

    /// <summary>
    /// Creates the.
    /// </summary>
    /// <param name="executable">The executable.</param>
    /// <param name="args">The args.</param>
    /// <returns>A Command.</returns>
    public static Command Create(string executable, params string[] args)
    {
        return Create(executable, ArgumentEscaper.EscapeAndConcatenateArgArrayForProcessStart(args));
    }

    /// <summary>
    /// Creates the.
    /// </summary>
    /// <param name="executable">The executable.</param>
    /// <param name="args">The args.</param>
    /// <returns>A Command.</returns>
    public static Command Create(string executable, IEnumerable<string> args)
    {
        return Create(executable, ArgumentEscaper.EscapeAndConcatenateArgArrayForProcessStart(args));
    }

    /// <summary>
    /// Creates the.
    /// </summary>
    /// <param name="executable">The executable.</param>
    /// <param name="args">The args.</param>
    /// <returns>A Command.</returns>
    public static Command Create(string executable, string args)
    {
        ResolveExecutablePath(ref executable, ref args);

        return new Command(executable, args);
    }

    /// <summary>
    /// Resolves the executable path.
    /// </summary>
    /// <param name="executable">The executable.</param>
    /// <param name="args">The args.</param>
    private static void ResolveExecutablePath(ref string executable, ref string args)
    {
        foreach(var suffix in RunnableSuffixes)
        {
            var fullExecutable = Path.GetFullPath(Path.Combine(
                                    AppContext.BaseDirectory, executable + suffix));

            if(File.Exists(fullExecutable))
            {
                executable = fullExecutable;

                // In priority order we've found the best runnable extension, so break.
                break;
            }
        }

        // On Windows, we want to avoid using "cmd" if possible (it mangles the colors, and a bunch of other things)
        // So, do a quick path search to see if we can just directly invoke it
        var useCmd = ShouldUseCmd(executable);

        if(useCmd)
        {
            var comSpec = System.Environment.GetEnvironmentVariable("ComSpec");

            // cmd doesn't like "foo.exe ", so we need to ensure that if
            // args is empty, we just run "foo.exe"
            if(!string.IsNullOrEmpty(args))
            {
                executable = (executable + " " + args).Replace("\"", "\\\"");
            }

            args = $"/C \"{executable}\"";
            executable = comSpec!;
        }
    }

    /// <summary>
    /// Shoulds the use cmd.
    /// </summary>
    /// <param name="executable">The executable.</param>
    /// <returns>A bool.</returns>
    private static bool ShouldUseCmd(string executable)
    {
        if(OperatingSystem.IsWindows())
        {
            var extension = Path.GetExtension(executable);
            if(!string.IsNullOrEmpty(extension))
            {
                return !string.Equals(extension, ".exe", StringComparison.Ordinal);
            }
            else if(executable.Contains(Path.DirectorySeparatorChar))
            {
                // It's a relative path without an extension
                if(File.Exists(executable + ".exe"))
                {
                    // It refers to an exe!
                    return false;
                }
            }
            else
            {
                // Search the path to see if we can find it
                foreach(var path in System.Environment.GetEnvironmentVariable("PATH")!.Split(Path.PathSeparator))
                {
                    var candidate = Path.Combine(path, executable + ".exe");
                    if(File.Exists(candidate))
                    {
                        // We found an exe!
                        return false;
                    }
                }
            }

            // It's a non-exe :(
            return true;
        }

        // Non-windows never uses cmd
        return false;
    }

    /// <summary>
    /// Environments the.
    /// </summary>
    /// <param name="env">The env.</param>
    /// <returns>A Command.</returns>
    public Command Environment(IDictionary<string, string> env)
    {
        if(env == null)
        {
            return this;
        }

        foreach(var item in env)
        {
            Process.StartInfo.Environment[item.Key] = item.Value;
        }

        return this;
    }

    /// <summary>
    /// Environments the.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>A Command.</returns>
    public Command Environment(string key, string value)
    {
        Process.StartInfo.Environment[key] = value;
        return this;
    }

    /// <summary>
    /// Executes the.
    /// </summary>
    /// <returns>A CommandResult.</returns>
    public CommandResult Execute()
    {
        return Execute(false);
    }

    /// <summary>
    /// Starts the.
    /// </summary>
    /// <returns>A Command.</returns>
    public Command Start()
    {
        ThrowIfRunning();

        _stopwatch = StopwatchValue.StartNew();

        _running = true;

        if(Process.StartInfo.RedirectStandardOutput)
        {
            Process.StartInfo.StandardOutputEncoding = Encoding.UTF8;

            Process.OutputDataReceived += (sender, args) =>
            {
                ProcessData(args.Data, _stdOutCapture);
            };
        }

        if(Process.StartInfo.RedirectStandardError)
        {
            Process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            Process.ErrorDataReceived += (sender, args) =>
            {
                ProcessData(args.Data, _stdErrCapture);
            };
        }

        Process.EnableRaisingEvents = true;

        ReportExecBegin();

        // Retry if we hit ETXTBSY due to Linux race
        // https://github.com/dotnet/runtime/issues/58964
        for(var i = 0; ; i++)
        {
            try
            {
                Process.Start();
                break;
            }
            catch(Win32Exception e) when(i < 4 && e.Message.Contains("Text file busy"))
            {
                Thread.Sleep(i * 20);
            }
        }

        if(Process.StartInfo.RedirectStandardOutput)
        {
            Process.BeginOutputReadLine();
        }

        if(Process.StartInfo.RedirectStandardError)
        {
            Process.BeginErrorReadLine();
        }

        return this;
    }

    /// <summary>
    /// Wait for the command to exit and dispose of the underlying process.
    /// </summary>
    /// <param name="expectedToFail">Whether or not the command is expected to fail (non-zero exit code)</param>
    /// <param name="timeoutMilliseconds">Time in milliseconds to wait for the command to exit</param>
    /// <returns>Result of the command</returns>
    public CommandResult WaitForExit(bool expectedToFail, int timeoutMilliseconds = Timeout.Infinite)
    {
        ReportExecWaitOnExit();

        int exitCode;
        if(!Process.WaitForExit(timeoutMilliseconds))
        {
            exitCode = -1;
        }
        else
        {
            exitCode = Process.ExitCode;
        }

        ReportExecEnd(exitCode, expectedToFail);

        Process.Dispose();

        return new CommandResult(
            Process.StartInfo.FileName,
            Process.StartInfo.Arguments,
            exitCode,
            _stdOutCapture?.GetStringBuilder()?.ToString(),
            _stdErrCapture?.GetStringBuilder()?.ToString(),
            _stopwatch.GetElapsedTime());
    }

    /// <summary>
    /// Execute the command and wait for it to exit.
    /// </summary>
    /// <param name="expectedToFail">Whether or not the command is expected to fail (non-zero exit code)</param>
    /// <returns>Result of the command</returns>
    public CommandResult Execute(bool expectedToFail)
    {
        // Clear out any enabling of dump creation if failure is expected
        if(expectedToFail)
        {
            EnvironmentVariable("COMPlus_DbgEnableMiniDump", null);
            EnvironmentVariable("DOTNET_DbgEnableMiniDump", null);
        }

        Start();

        return WaitForExit(expectedToFail);
    }

    /// <summary>
    /// Workings the directory.
    /// </summary>
    /// <param name="projectDirectory">The project directory.</param>
    /// <returns>A Command.</returns>
    public Command WorkingDirectory(string projectDirectory)
    {
        Process.StartInfo.WorkingDirectory = projectDirectory;
        return this;
    }

    /// <summary>
    /// Environments the variable.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>A Command.</returns>
    public Command EnvironmentVariable(string name, string? value)
    {
        value ??= "";

        Process.StartInfo.Environment[name] = value;

        return this;
    }

    /// <summary>
    /// Removes the environment variable.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A Command.</returns>
    public Command RemoveEnvironmentVariable(string name)
    {
        Process.StartInfo.Environment.Remove(name);
        return this;
    }

    /// <summary>
    /// 捕获标准输出
    /// </summary>
    /// <returns></returns>
    public Command CaptureStdOut()
    {
        ThrowIfRunning();
        Process.StartInfo.RedirectStandardOutput = true;
        _stdOutCapture = new StringWriter();
        return this;
    }

    /// <summary>
    /// Captures the std err.
    /// </summary>
    /// <returns>A Command.</returns>
    public Command CaptureStdErr()
    {
        ThrowIfRunning();
        Process.StartInfo.RedirectStandardError = true;
        _stdErrCapture = new StringWriter();
        return this;
    }

    /// <summary>
    /// Formats the process info.
    /// </summary>
    /// <param name="info">The info.</param>
    /// <param name="includeWorkingDirectory">If true, include working directory.</param>
    /// <returns>A string.</returns>
    private static string FormatProcessInfo(ProcessStartInfo info, bool includeWorkingDirectory)
    {
        var prefix = includeWorkingDirectory ?
            $"{info.WorkingDirectory}> {info.FileName}" :
            info.FileName;

        if(string.IsNullOrWhiteSpace(info.Arguments))
        {
            return prefix;
        }

        return prefix + " " + info.Arguments;
    }

    /// <summary>
    /// Reports the exec begin.
    /// </summary>
    private void ReportExecBegin()
    {
        CommandReporter.BeginSection("EXEC", FormatProcessInfo(Process.StartInfo, includeWorkingDirectory: false));
    }

    /// <summary>
    /// Reports the exec wait on exit.
    /// </summary>
    private void ReportExecWaitOnExit()
    {
        CommandReporter.SectionComment("EXEC", $"Waiting for process {Process.Id} to exit...");
    }

    /// <summary>
    /// Reports the exec end.
    /// </summary>
    /// <param name="exitCode">The exit code.</param>
    /// <param name="fExpectedToFail">If true, f expected to fail.</param>
    private void ReportExecEnd(int exitCode, bool fExpectedToFail)
    {
        var success = exitCode == 0;
        var msgExpectedToFail = "";

        if(fExpectedToFail)
        {
            success = !success;
            msgExpectedToFail = "failed as expected and ";
        }

        var message = $"{FormatProcessInfo(Process.StartInfo, includeWorkingDirectory: !success)} {msgExpectedToFail}exited with {exitCode}";

        CommandReporter.EndSection(
            "EXEC",
            success ? message.Green() : message.Red().Bold(),
            success);
    }

    /// <summary>
    /// Throws the if running.
    /// </summary>
    /// <param name="memberName">The member name.</param>
    private void ThrowIfRunning([CallerMemberName] string? memberName = null)
    {
        if(_running)
        {
            throw new InvalidOperationException($"Unable to invoke {memberName} after the command has been run");
        }
    }

    /// <summary>
    /// Processes the data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="capture">The capture.</param>
    private static void ProcessData(string? data, StringWriter? capture)
    {
        if(data == null)
        {
            return;
        }

        capture?.WriteLine(data);
    }
}
