namespace OneI;

internal static partial class Interop
{
    public static partial class Libc
    {
        internal const string Library = "libc";

        [LibraryImport(Library, EntryPoint = "getppid")]
        internal static partial int GetParentPid();
    }
}
