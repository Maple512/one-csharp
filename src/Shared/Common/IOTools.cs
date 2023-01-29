namespace OneI;

internal static class IOTools
{
    public static void EnsureExistedDirectory(string path)
    {
        if(path is not { Length: > 0 })
        {
            throw new ArgumentNullException(nameof(path));
        }

        var directory = Path.GetDirectoryName(path)!;

        if(!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public static void EnsureEmptyDirectory(string path)
    {
        if(path is not { Length: > 0 })
        {
            throw new ArgumentNullException(nameof(path));
        }

        var directory = Path.GetDirectoryName(path)!;

        if(!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);

            return;
        }

        Directory.Delete(directory, true);

        Directory.CreateDirectory(directory);
    }
}
