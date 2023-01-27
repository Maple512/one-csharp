namespace OneI.Logable.Internal;

internal readonly struct FileItem
{
    public FileItem(string fullPath)
    {
        CreatedAt = DateTime.Now;
        FullPath = fullPath;
    }

    public string FullPath { get; }

    public DateTime CreatedAt { get; }
}
