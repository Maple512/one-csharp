namespace OneT.Common;

using System.IO;
using ObjectLayoutInspector;

public static class TestTools
{
    public static void PrintToFile(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null)
    {
        var name = Path.GetFileNameWithoutExtension(file);

        var fullname = Path.Combine(GetCSProjectDirecoty(file), $"./Logs/{name}@{member}.txt");

        var path = Path.GetDirectoryName(fullname)!;

        if(Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        File.AppendAllText(fullname, message, Encoding.UTF8);
    }

    /// <summary>
    /// 打印内存中类或结构的数据字段的物理布局
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void PrintLayoutToFile<T>([CallerFilePath] string? file = null, [CallerMemberName] string? member = null)
    {
        PrintToFile($"[{DateTime.Now:HH:mm}] {TypeLayout.GetLayout<T>()}", file, member);
    }

    /// <summary>
    /// 打印内存中类或结构的数据字段的物理布局
    /// </summary>
    public static void PrintLayoutToFile(Type type, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null)
    {
        PrintToFile($"[{DateTime.Now:HH:mm}] {TypeLayout.GetLayout(type)}", file, member);
    }

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

    /// <summary>
    /// Tries the get c s project direcoty.
    /// </summary>
    /// <param name="csprojDirectory">The csproj directory.</param>
    /// <param name="filePath">The file path.</param>
    /// <returns>A bool.</returns>
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
