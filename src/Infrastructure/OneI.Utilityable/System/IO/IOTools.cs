namespace System.IO;

using OneI;
/// <summary>
/// The i o tools.
/// </summary>

[StackTraceHidden]
[DebuggerStepThrough]
public static class IOTools
{
    /// <summary>
    /// Gets the relative path.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <param name="file">The file.</param>
    /// <returns>A string.</returns>
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

    /// <summary>
    /// Ensures the directory existed.
    /// </summary>
    /// <param name="filePath">The file path.</param>
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
