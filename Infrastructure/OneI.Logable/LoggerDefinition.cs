namespace OneI.Logable;

public static class LoggerDefinition
{
    public static class PropertyNames
    {
        public const string Level = nameof(Level);

        public const string NewLine = nameof(NewLine);

        public const string Message = nameof(Message);

        public const string Timestamp = nameof(Timestamp);

        public const string Exception = nameof(Exception);

        public const string Properties = nameof(Properties);

        /// <summary>
        /// called member name
        /// </summary>
        public const string Member = nameof(Member);

        /// <summary>
        /// called file path
        /// </summary>
        public const string File = nameof(File);

        /// <summary>
        /// called file line number
        /// </summary>
        public const string Line = nameof(Line);

        /// <summary>
        /// The name of the property included in the emitted log events
        /// when <code>ForContext&lt;T&gt;()</code> and overloads are
        /// applied.
        /// </summary>
        public const string SourceContext = nameof(SourceContext);
    }

    public static class Formatters
    {
        /// <summary>
        /// 字面量，按原样输出
        /// </summary>
        public const char Literal = 'l';

        /// <summary>
        /// 如可能，呈现为JSON字符串
        /// </summary>
        public const char Json = 'j';

        public const char Upper = 'u';

        public const char Lower = 'w';

        public const char Title = 't';
    }
}
