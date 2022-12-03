namespace OneI.Logable;

public static class LoggerConstants
{
    internal static class PropertyNames
    {
        public const string Level = nameof(Level);

        public const string NewLine = nameof(NewLine);

        public const string Message = nameof(Message);

        public const string Timestamp = nameof(Timestamp);

        public const string Exception = nameof(Exception);

        public const string MemberName = nameof(MemberName);

        public const string FilePath = nameof(FilePath);

        public const string LineNumber = nameof(LineNumber);

        public const string Properties = nameof(Properties);

        public const string SourceContext = nameof(SourceContext);
    }

    internal static class Formatters
    {
        public const char Render_As_String = '$';

        public const char Render_As_Structure = '@';

        public const char Align_Separator = ',';

        public const char Format_Separator = ':';

        public const char Open_Separator = '{';

        public const char Close_Separator = '}';

        public const char Level_Upper = 'u';

        public const char Level_Lower = 'w';

        public const char Level_Narmal = 'n';
    }
}
