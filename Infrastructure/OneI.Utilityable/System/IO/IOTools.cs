namespace System.IO;

using OneI;

#if NET7_0_OR_GREATER

using System.Diagnostics.CodeAnalysis;

[StackTraceHidden]
#endif
[DebuggerStepThrough]
public static class IOTools
{
#if NET7_0_OR_GREATER

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

#elif NETSTANDARD
    public static string GetRelativePath(string directory, string file)
    {
        var fullPath = Path.Combine(directory, file);

        if(directory.EndsWith(char.ToString(Path.DirectorySeparatorChar))
            || directory.EndsWith(char.ToString(Path.AltDirectorySeparatorChar)))
        {
            return fullPath.Substring(directory.Length, fullPath.Length - directory.Length);// [directory.Length..];
        }

        return fullPath.Substring(directory.Length + 1, fullPath.Length - (directory.Length + 1));
    }
#endif

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
