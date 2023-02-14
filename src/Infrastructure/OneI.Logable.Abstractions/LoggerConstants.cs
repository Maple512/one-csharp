namespace OneI.Logable;

public static class LoggerConstants
{
    internal const string MessagePlaceHolder = $"{{{Propertys.Message}}}";

    public static class Propertys
    {
        public const int MaxCount = 100;

        public const string Level = nameof(Level);

        public const string NewLine = nameof(NewLine);

        public const string Message = nameof(Message);

        public const string Timestamp = nameof(Timestamp);

        public const string Exception = nameof(Exception);

        public const string Member = nameof(Member);

        public const string File = nameof(File);

        public const string FileName = nameof(FileName);

        public const string FileNameWithoutExtension = nameof(FileNameWithoutExtension);

        public const string FileExtension = nameof(FileExtension);

        public const string Line = nameof(Line);

        public const string SourceContext = nameof(SourceContext);

        public const string DefaultSourceContext = "Default";

        public const string EventId = nameof(EventId);

        public const int DefaultEventId = 0;
    }

    internal static class Formatters
    {
        public const char UpperCase = 'u';
        public const char LowerCase = 'l';
    }
}
