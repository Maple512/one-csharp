namespace OneI.Logable.Templates;

internal static class TemplateConstants
{
    public static class Property
    {
        public const int NameLengthLimit   = 32;
        public const int FormatLengthLimit = 32;
        public const int IndexLengthLimit  = 3;
        public const int IndexNumericLimit = 99;
        public const int AlignLengthLimit  = 3;
        public const int AlignNumericLimit = 99;

        public static string PropertyNameValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
        public static string IndentStringValidChars = "0123456789";
        public static string AlignStringValidChars  = "0123456789-";
        public static string Null                   = "[Null]";
    }

    public static class Formatters
    {
        public const char Align_Separator = ',';

        public const char Format_Separator = ':';

        public const char Indent_Separator = '\'';

        public const char Open_Separator = '{';

        public const char Close_Separator = '}';

        public const string UpperCase = "u";

        public const string LowerCase = "l";
    }
}
