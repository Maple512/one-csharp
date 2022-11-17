namespace System.IO;

using System.Diagnostics;

[StackTraceHidden]
[DebuggerStepThrough]
public static class IOTools
{
    public static void EnsureDirectoryNotExisted(string path)
    {
        if(Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public static string GetRelativePath(string directory, string file)
    {
        var fullPath = Path.Combine(directory, file);
        if(directory.EndsWith(Path.DirectorySeparatorChar)
            || directory.EndsWith(Path.AltDirectorySeparatorChar))
        {
            return fullPath[directory.Length..];
        }

        return fullPath[(directory.Length + 1)..];
    }

    public static void CreateDirectory(string directory)
    {
        if(Directory.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }
    }
}
