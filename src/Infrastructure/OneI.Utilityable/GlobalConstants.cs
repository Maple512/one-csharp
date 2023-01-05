namespace OneI;

public static class GlobalConstants
{
    // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/String.cs$L31
    public const int StringMaxLength = 0x3FFFFFDF;

    // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/String.Manipulation.cs#L539
    public const int ArrayPoolMinimumLength = 256;
}
