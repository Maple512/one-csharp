namespace OneI;

using System.IO;

internal static class IOTools
{
    public static void EnsureFileDirectory(string file, bool empty = false)
    {
        if(file is not { Length: > 0 })
        {
            throw new ArgumentNullException(nameof(file));
        }

        var dire = GetDirectory(file, false);

        if(!dire.Exists)
        {
            dire.Create();
        }
        else if(dire.Exists && empty)
        {
            dire.Delete(true);
            dire.Create();
        }
    }

    public static void EnsureDirectory(string directory, bool empty = false)
    {
        if(directory is not { Length: > 0 })
        {
            throw new ArgumentNullException(nameof(directory));
        }

        var dire = GetDirectory(directory, true);

        if(!dire.Exists)
        {
            dire.Create();
        }
        else if(dire.Exists && empty)
        {
            dire.Delete(true);
            dire.Create();
        }
    }

    private static DirectoryInfo GetDirectory(string path, bool mayByDir)
    {
        var attributes = (int)new FileInfo(path).Attributes;

        // 如果文件或文件夹都不存在，则 attributes = -1
        // 16 is FileAttributes.Directory
        if((attributes == -1 && mayByDir) && (attributes & 16) != 0)
        {
            return new DirectoryInfo(path);
        }
        else
        {
            return new DirectoryInfo(Path.GetDirectoryName(path)!);
        }
    }
}
