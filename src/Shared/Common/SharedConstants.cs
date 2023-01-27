namespace OneI;

internal static partial class SharedConstants
{
    public const int ArrayMaxLength = 0X7FFFFFC7;
    public const int StringFormatMinimumLength = 256;

    public const string DEBUG = nameof(DEBUG);

    public static class OSPlatformName
    {
        public const string Windows = "windows";
        public const string Linux = "linux";
        public const string Android = "android";
        public const string Browser = "browser";
        public const string iOS = "ios";
        public const string MacCatalyst = "maccatalyst";
        public const string tvOS = "tvos";
    }
}