namespace OneI.Hostable.Internal;

internal static class Win32
{
    public static unsafe Process? GetParentProcess()
    {
        nint snapshotHandle = 0;
        try
        {
            snapshotHandle = Interop.Kernel32.CreateToolhelp32Snapshot(Interop.Kernel32.SnapshotFlags.Process, 0);

            Interop.Kernel32.PROCESSENTRY32 processEntry = default;
            processEntry.dwSize = sizeof(Interop.Kernel32.PROCESSENTRY32);
            if(Interop.Kernel32.Process32First(snapshotHandle, &processEntry))
            {
                var currentProcessId = Environment.ProcessId;

                do
                {
                    if(currentProcessId == processEntry.th32ProcessID)
                    {
                        return Process.GetProcessById(currentProcessId);
                    }
                } while(Interop.Kernel32.Process32Next(snapshotHandle, &processEntry));
            }
        }
        catch(Exception) { }
        finally
        {
            _ = Interop.Kernel32.CloseHandle(snapshotHandle);
        }

        return null;
    }
}
