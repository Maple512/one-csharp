namespace OneT.Common;

using System.IO;

public static class TestTools
{
    /// <summary>
    /// 获取项目csporj文件所在目录
    /// </summary>
    /// <param name="filePath">调用该方法的文件地址</param>
    /// <returns></returns>
    public static string GetCSProjectDirecoty([CallerFilePath] string? filePath = null)
    {
        if(TryGetCSProjectDirecoty(out var csprojDirectory, filePath) == false)
        {
            return Path.GetDirectoryName(filePath)! ?? Directory.GetCurrentDirectory();
        }

        return csprojDirectory;
    }

    private static bool TryGetCSProjectDirecoty([NotNullWhen(true)] out string? csprojDirectory, [CallerFilePath] string? filePath = null)
    {
        var directory = Path.GetDirectoryName(filePath);
        csprojDirectory = null;

        if(directory.IsNullOrWhiteSpace())
        {
            return false;
        }

        var projFile = Directory.GetFiles(directory, "*.csproj")
             .FirstOrDefault();

        if(projFile is null)
        {
            return TryGetCSProjectDirecoty(out csprojDirectory, directory!);
        }

        csprojDirectory = Path.GetDirectoryName(projFile)!;

        return true;
    }
}
