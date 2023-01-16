namespace OneI.Hostable;

using System.Globalization;

public static class SystemdHelper
{
    private static bool? _isSystemdService;

    public static bool IsSystemdService()
        => _isSystemdService ??= GetIsSystemdService();

    private static bool GetIsSystemdService()
    {
        if(Environment.OSVersion.Platform != PlatformID.Unix)
        {
            return false;
        }

        var processId = Environment.ProcessId;

        if(processId == 1)
        {
            return Environment.GetEnvironmentVariable("NOTIFY_SOCKET") is { Length: > 0 }
            || Environment.GetEnvironmentVariable("LISTEN_PID") is { Length: > 0 };
        }

        try
        {
            var parentId = Interop.Libc.GetParentPid();

            var ppidStr = parentId.ToString(NumberFormatInfo.InvariantInfo);

            var comm = File.ReadAllBytes($"/proc/{ppidStr}/comm");

            return comm.AsSpan().SequenceEqual("systemd\n"u8);
        }
        catch { }

        return false;
    }
}
