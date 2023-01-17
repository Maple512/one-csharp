namespace OneI.Logable.Internal;

using Microsoft.Win32.SafeHandles;

internal readonly struct FileItem
{
    public FileItem(SafeFileHandle file, string fullPath)
    {
        CreatedAt = DateTime.Now;
        File = file;
        FullPath = fullPath;
    }

    public SafeFileHandle File { get; }

    public string FullPath { get; }

    public DateTime CreatedAt { get; }
}
