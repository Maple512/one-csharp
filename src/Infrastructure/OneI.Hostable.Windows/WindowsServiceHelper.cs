namespace OneI.Hostable;

using OneI.Hostable.Internal;

public static class WindowsServiceHelper
{
    public static bool IsWindowsService()
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == false)
        {
            return false;
        }

        var parentProcess = Win32.GetParentProcess();
        if(parentProcess == null)
        {
            return false;
        }

        return "services".AsSpan().Equals(parentProcess.ProcessName, StringComparison.OrdinalIgnoreCase);
    }
}
