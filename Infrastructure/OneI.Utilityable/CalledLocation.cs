namespace OneI;

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
