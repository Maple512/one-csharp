namespace System.IO;

using System.Diagnostics;
/// <summary>
/// The directory tools.
/// </summary>

[StackTraceHidden]
[DebuggerStepThrough]
public class DirectoryTools
{
    /// <summary>
    /// Creates the directory.
    /// </summary>
    /// <param name="directory">The directory.</param>
    public static void CreateDirectory(string directory)
    {
        if(Directory.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }
    }
}
