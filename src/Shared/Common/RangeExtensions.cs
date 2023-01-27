namespace OneI;

internal static class RangeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValid(this Range range)
    {
        return range.Start.Value <= range.End.Value;
    }
}
