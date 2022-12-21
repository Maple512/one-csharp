namespace System.IO;

using OneI;

[StackTraceHidden]
[DebuggerStepThrough]
public static class IOTools
{
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

    public static void EnsureDirectoryExisted(string filePath)
    {
        Check.NotNullOrWhiteSpace(filePath);

        var directory = Path.GetDirectoryName(filePath);
        if(Directory.Exists(directory) == false
            && directory.NotNullOrWhiteSpace())
        {
            Directory.CreateDirectory(directory);
        }
    }
}
