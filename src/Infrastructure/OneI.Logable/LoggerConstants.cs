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

        public const string FileName = nameof(FileName);

        public const string FileNameWithoutExtension = nameof(FileNameWithoutExtension);

        public const string FileExtension = nameof(FileExtension);

        public const string LineNumber = nameof(LineNumber);

        public const string SourceContext = nameof(SourceContext);
    }

    internal static class Formatters
    {
        public const string UpperCase = "u";

        public const string LowerCase = "l";
    }
}
