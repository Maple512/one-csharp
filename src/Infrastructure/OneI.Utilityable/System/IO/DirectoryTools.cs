namespace System.IO;

using System.Diagnostics;

[StackTraceHidden]
[DebuggerStepThrough]
public class DirectoryTools
{
    public static void CreateDirectory(string directory)
    {
        if(Directory.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }
    }
}
