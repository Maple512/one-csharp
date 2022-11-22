namespace OneT.Common;

using System.IO;

public static class TestTools
{
    /// <summary>
    /// 获取<paramref name="file"/>在项目中的完整路径
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static string GetFilePathWithinProject(string file)
    {
        var projectPath = Directory.CreateDirectory(Directory.GetCurrentDirectory()).Parent!.Parent!.Parent!.FullName;

        var path = Path.Combine(projectPath, file);

        EnsureDirectoryExisted(Path.GetDirectoryName(path)!);

        return Path.GetFullPath(path);
    }

    public static void EnsureDirectoryExisted(string directory)
    {
        if(Directory.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }
    }
}
