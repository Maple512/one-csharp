namespace System;

internal static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotNullOrEmpty(this string? str) => string.IsNullOrEmpty(str) == false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str) == false;
}
