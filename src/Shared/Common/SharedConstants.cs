namespace OneI;

internal static partial class SharedConstants
{
    public const string DEBUG = nameof(DEBUG);

    public static class NewLine
    {
#if WINDOWS
        public const bool CRLF = true;
#else
        public const bool CRLF = false;
#endif
    }

    public static class OSPlatform
    {
        public const string Windows = "windows";
        public const string Linux = "linux";
        public const string Android = "android";
        public const string Browser = "browser";
        public const string iOS = "ios";
        public const string MacCatalyst = "maccatalyst";
        public const string tvOS = "tvos";
    }

    public static class Array
    {
        /// <summary>
        /// 数组最大长度
        /// <para><see href="https://github.com/dotnet/runtime/blob/06bf1c6f6e54994bfecbadf5bc318d97dcd711cc/src/libraries/System.Private.CoreLib/src/System/Array.cs#L2067"/></para>
        /// </summary>
        public const int ArrayMaxLength = 0X7FFFFFC7;

        /// <summary>
        /// <c>stackalloc</c>语法分配阈值，当超过该阈值时，请考虑其他方式分配数组
        /// </summary>
        /// <remarks>
        /// 示例 
        /// <code>
        ///     Span&lt;byte&gt; buffer = userInput > StackallocByteThreshold ? new byte[userInput] : stackalloc byte[StackallocByteThreshold];
        /// </code>
        /// </remarks>
        public const int StackallocByteThreshold = 256;
    }

    public static class Json
    {
        public const byte OpenBrace = (byte)'{';
        public const byte CloseBrace = (byte)'}';
        public const byte OpenBracket = (byte)'[';
        public const byte CloseBracket = (byte)']';
        public const byte Space = (byte)' ';
        public const byte CarriageReturn = (byte)'\r';
        public const byte LineFeed = (byte)'\n';
        public const byte Tab = (byte)'\t';
        public const byte ListSeparator = (byte)',';
        public const byte KeyValueSeparator = (byte)':';
        public const byte Quote = (byte)'"';
        public const byte BackSlash = (byte)'\\';
        public const byte Slash = (byte)'/';
        public const byte BackSpace = (byte)'\b';
        public const byte FormFeed = (byte)'\f';
        public const byte Asterisk = (byte)'*';
        public const byte Colon = (byte)':';
        public const byte Period = (byte)'.';
        public const byte Plus = (byte)'+';
        public const byte Hyphen = (byte)'-';
        public const byte UtcOffsetToken = (byte)'Z';
        public const byte TimePrefix = (byte)'T';

        // In the worst case, an ASCII character represented as a single utf-8 byte could expand 6x when escaped.
        // For example: '+' becomes '\u0043'
        // Escaping surrogate pairs (represented by 3 or 4 utf-8 bytes) would expand to 12 bytes (which is still <= 6x).
        // The same factor applies to utf-16 characters.
        public const int MaxExpansionFactorWhileEscaping = 6;

        public const int StackallocByteThreshold = 256;
        public const int StackallocCharThreshold = StackallocByteThreshold / 2;
    }
}
