namespace System.IO;

using System.Diagnostics;

#if NET7_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
[StackTraceHidden]
#endif
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
