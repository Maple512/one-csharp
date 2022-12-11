namespace System.Diagnostics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

public class ProcessActuator
{
    [DllImport("libc", SetLastError = true, EntryPoint = "kill")]
    private static extern int sys_kill(int pid, int sig);

    private static readonly bool _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    // source: https://github.com/dotnet/tye/blob/bb49c161641b9f182cba61641149006811f60dc2/src/Microsoft.Tye.Core/ProcessUtil.cs

    /// <summary>
    /// 使用指定的参数运行一个进程
    /// </summary>
    /// <param name="paramter">参数</param>
    /// <param name="continueWithErrors">遇到错误是否继续</param>
    /// <param name="token">取消令牌</param>
    /// <returns></returns>
    public static async ValueTask<ProcessResult> RunAsync(
        ProcessParameter paramter,
        bool continueWithErrors = true,
        CancellationToken token = default)
    {
        var arguments = paramter.Arguments.JoinAsString(' ');

        using var process = new Process
        {
            StartInfo =
            {
                FileName = paramter.FileName,
                Arguments = arguments,
                WorkingDirectory = paramter.WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                //StandardOutputEncoding =  Encoding.UTF8,
                //StandardErrorEncoding =  Encoding.UTF8,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            },
            EnableRaisingEvents = true,
        };

        if(paramter.Environments.Any())
        {
            foreach(var env in paramter.Environments)
            {
                process.StartInfo.Environment.Add(env);
            }
        }

        if(paramter.EnvironmentsToRemove.Any())
        {
            foreach(var env in paramter.EnvironmentsToRemove)
            {
                process.StartInfo.Environment.Remove(env);
            }
        }

        process.OutputDataReceived += (_, e) =>
        {
            paramter.OutputReceiver?.Invoke(true, e.Data);

            paramter.OutputBuilder?.AppendLine(e.Data);

            Debug.WriteLine(e.Data, "Output Data");
        };

        process.ErrorDataReceived += (_, e) =>
        {
            paramter.OutputReceiver?.Invoke(false, e.Data);

            paramter.OutputBuilder?.AppendLine(e.Data);

            Debug.WriteLine(e.Data, "Error Data");
        };

        var processLifetimeTask = new TaskCompletionSource<ProcessResult>();

        process.Exited += (_, e) =>
        {
            if(continueWithErrors && process.ExitCode != 0)
            {
                processLifetimeTask.TrySetException(new InvalidOperationException($"Command {paramter.FileName} {paramter.Arguments.JoinAsString(' ')} returned exit code: {process.ExitCode}"));
            }
            else
            {
                processLifetimeTask.TrySetResult(new ProcessResult(
                    process.Id,
                    process.ExitCode,
                    process.TotalProcessorTime));
            }
        };

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        var cancelledTcs = new TaskCompletionSource<object?>();

#if NET7_0_OR_GREATER
        await using var registration = token.Register(() => cancelledTcs.TrySetResult(null));
#else
        using var registration = token.Register(() => cancelledTcs.TrySetResult(null));
#endif
        var result = await Task.WhenAny(processLifetimeTask.Task, cancelledTcs.Task);

        if(result == cancelledTcs.Task)
        {
            if(!_isWindows)
            {
                sys_kill(process.Id, sig: 2); // SIGINT
            }
            else
            {
                if(!process.CloseMainWindow())
                {
                    process.Kill();
                }
            }

            if(!process.HasExited)
            {
                var cancel = new CancellationTokenSource();

                await Task.WhenAny(processLifetimeTask.Task, Task.Delay(TimeSpan.FromSeconds(5), cancel.Token));

                cancel.Cancel();

                if(!process.HasExited)
                {
                    process.Kill();
                }
            }
        }

        return await processLifetimeTask.Task;
    }
}
