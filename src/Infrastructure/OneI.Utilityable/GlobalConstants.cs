namespace OneI;

public static class GlobalConstants
{
    /// <summary>
    /// Gets the maximum number of elements that may be contained in an array.
    /// <para>source: <see href="https://github.com/dotnet/runtime/blob/6009a1064ccfc2bd9aeb96c9247b60cf6352198d/src/libraries/System.Private.CoreLib/src/System/Array.cs#L2069"/></para>
    /// <para>Keep in sync with `inline SIZE_T MaxArrayLength()` from gchelpers and HashHelpers.MaxPrimeArrayLength.</para>
    /// </summary>
    /// <returns>The maximum count of elements allowed in any array.</returns>
    /// <remarks>
    /// <para>
    /// This property represents a runtime limitation, the maximum number of elements (not bytes)
    /// the runtime will allow in an array. There is no guarantee that an allocation under this
    /// length will succeed, but all attempts to allocate a larger array will fail.
    /// </para>
    /// <para>
    /// This property only applies to single-dimension, zero-bound (SZ) arrays. <see cref="Array.Length"/>
    /// property may return larger value than this property for multi-dimensional arrays.
    /// </para>
    /// </remarks>
    public const uint ArrayMaxLength = 0x7FFFFFC7;

    /// <summary>
    /// Maximum length allowed for a string.
    /// <para>source: <see href="https://github.com/dotnet/runtime/blob/6009a1064ccfc2bd9aeb96c9247b60cf6352198d/src/libraries/System.Private.CoreLib/src/System/String.cs#L31"/></para>
    /// </summary>
    /// <remarks>
    /// Keep in sync with AllocateString in gchelpers.cpp.
    /// </remarks>
    public const int StringMaxLength = 0x3FFFFFDF;

    /// <summary>
    /// string format minimum length: <see langword="256"/>
    /// <para>source: <see href="https://github.com/dotnet/runtime/blob/6009a1064ccfc2bd9aeb96c9247b60cf6352198d/src/libraries/System.Private.CoreLib/src/System/String.Manipulation.cs#L539"/></para>
    /// </summary>
    public const int StringFormatMinimumLength = 256;
}
