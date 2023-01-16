namespace OneI;

internal static partial class Interop
{
    public static partial class Kernel32
    {
        private const int MAX_PATH = 260;

        private const string Library = "kernel32.dll";

        [Flags]
        internal enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            All = (HeapList | Process | Thread | Module),
            Inherit = 0x80000000,
            NoHeaps = 0x40000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal unsafe struct PROCESSENTRY32
        {
            internal int dwSize;
            internal int cntUsage;
            internal int th32ProcessID;
            internal IntPtr th32DefaultHeapID;
            internal int th32ModuleID;
            internal int cntThreads;
            internal int th32ParentProcessID;
            internal int pcPriClassBase;
            internal int dwFlags;
            internal fixed char szExeFile[MAX_PATH];
        }

        // https://docs.microsoft.com/windows/desktop/api/tlhelp32/nf-tlhelp32-createtoolhelp32snapshot
        [LibraryImport(Library, SetLastError = true)]
        internal static partial nint CreateToolhelp32Snapshot(SnapshotFlags dwFlags, uint th32ProcessID);

        // https://docs.microsoft.com/windows/desktop/api/tlhelp32/nf-tlhelp32-process32first
        [LibraryImport(Library, EntryPoint = "Process32FirstW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static unsafe partial bool Process32First(nint hSnapshot, PROCESSENTRY32* lppe);

        // https://docs.microsoft.com/windows/desktop/api/tlhelp32/nf-tlhelp32-process32next
        [LibraryImport(Library, EntryPoint = "Process32NextW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static unsafe partial bool Process32Next(nint hSnapshot, PROCESSENTRY32* lppe);

        [LibraryImport(Library, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool CloseHandle(IntPtr handle);
    }
}
