namespace OneI;

using System.IO;

/// <summary>
/// 表示调用时的位置
/// </summary>
/// <param name="FilePath">文件全路径</param>
/// <param name="MemberName">方法名</param>
/// <param name="LineNumber">调用位置在文件中的行树</param>
public readonly record struct CalledLocation(string? FilePath, string? MemberName, int? LineNumber)
{
    public override string ToString()
    {
        return $"{FilePath}#L{LineNumber}@{MemberName}";
    }

    public string ToSortString()
    {
        return $"{Path.GetFileNameWithoutExtension(FilePath)}#L{LineNumber}@{MemberName}";
    }
}
