namespace OneI;

public readonly record struct CalledLocation(string? MemberName, string? FilePath, int? LineNumber)
{
    public override string ToString()
    {
        return $"{FilePath}#{LineNumber}@{MemberName}";
    }
}
