namespace OneI.Logable;
/// <summary>
/// The logger constants.
/// </summary>

public static class LoggerConstants
{
    /// <summary>
    /// The property names.
    /// </summary>
    internal static class PropertyNames
    {
        /// <summary>
        /// The level.
        /// </summary>
        public const string Level = nameof(Level);

        /// <summary>
        /// The new line.
        /// </summary>
        public const string NewLine = nameof(NewLine);

        /// <summary>
        /// The message.
        /// </summary>
        public const string Message = nameof(Message);

        /// <summary>
        /// The timestamp.
        /// </summary>
        public const string Timestamp = nameof(Timestamp);

        /// <summary>
        /// The exception.
        /// </summary>
        public const string Exception = nameof(Exception);

        /// <summary>
        /// The member name.
        /// </summary>
        public const string MemberName = nameof(MemberName);

        /// <summary>
        /// The file path.
        /// </summary>
        public const string FilePath = nameof(FilePath);

        /// <summary>
        /// The line number.
        /// </summary>
        public const string LineNumber = nameof(LineNumber);

        /// <summary>
        /// The source context.
        /// </summary>
        public const string SourceContext = nameof(SourceContext);
    }

    /// <summary>
    /// The formatters.
    /// </summary>
    internal static class Formatters
    {
        /// <summary>
        /// The render_ as_ string.
        /// </summary>
        public const char Render_As_String = '$';

        /// <summary>
        /// The render_ as_ structure.
        /// </summary>
        public const char Render_As_Structure = '@';

        /// <summary>
        /// The align_ separator.
        /// </summary>
        public const char Align_Separator = ',';

        /// <summary>
        /// The format_ separator.
        /// </summary>
        public const char Format_Separator = ':';

        /// <summary>
        /// The indent_ separator.
        /// </summary>
        public const char Indent_Separator = '\'';

        /// <summary>
        /// The open_ separator.
        /// </summary>
        public const char Open_Separator = '{';

        /// <summary>
        /// The close_ separator.
        /// </summary>
        public const char Close_Separator = '}';

        /// <summary>
        /// The upper case.
        /// </summary>
        public const string UpperCase = "u";

        /// <summary>
        /// The lower case.
        /// </summary>
        public const string LowerCase = "l";
    }
}
