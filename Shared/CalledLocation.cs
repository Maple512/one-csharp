namespace OneI;

using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 表示调用时的位置
/// </summary>
internal readonly struct CalledLocation : IEquatable<CalledLocation>
{
    /// <summary>
    /// 表示调用时的位置
    /// </summary>
    /// <param name="filePath">文件全路径</param>
    /// <param name="memberName">方法名</param>
    public CalledLocation(string? filePath, string? memberName, int? lineNumber)
    {
        FilePath = filePath ?? string.Empty;
        MemberName = memberName ?? string.Empty;
        LineNumber = lineNumber ?? 0;
    }

    /// <summary>文件全路径</summary>
    public string FilePath { get; }

    /// <summary>方法名</summary>
    public string MemberName { get; }

    /// <summary>调用位置在文件中的行树</summary>
    public int LineNumber { get; }

    public static bool operator !=(CalledLocation left, CalledLocation right)
    {
        return !(left == right);
    }

    public static bool operator ==(CalledLocation left, CalledLocation right)
    {
        return left.Equals(right);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<string>.Default.GetHashCode(FilePath) * -1521134295
            + EqualityComparer<string>.Default.GetHashCode(MemberName) * -1521134295
            + EqualityComparer<int?>.Default.GetHashCode(LineNumber);
    }

    public override bool Equals(object? obj)
    {
        return obj is CalledLocation other && Equals(other);
    }

    public bool Equals(CalledLocation other)
    {
        return EqualityComparer<string>.Default.Equals(FilePath, other.FilePath)
            && EqualityComparer<string>.Default.Equals(MemberName, other.MemberName)
            && EqualityComparer<int?>.Default.Equals(LineNumber, other.LineNumber);
    }

    public override string ToString()
    {
        return $"{FilePath}#L{LineNumber}@{MemberName}";
    }

    public string ToSortString()
    {
        return $"{Path.GetFileNameWithoutExtension(FilePath)}#L{LineNumber}@{MemberName}";
    }
}
