namespace OneI;

using System;
using System.IO;

[DebuggerStepThrough]
internal static partial class IOTools
{
    public static void EnsureEmptyDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);

        if(path.IsNullOrWhiteSpace() || !Directory.Exists(directory))
        {
            return;
        }

        Directory.Delete(directory, true);

        Directory.CreateDirectory(directory);
    }
}
