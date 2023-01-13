namespace OneI.Logable.Templatizations;

internal static class Constants
{
    public static class Property
    {
        public const int NameLengthLimit = 32;
        public const int FormatLengthLimit = 32;
        public const int IndexLengthLimit = 3;
        public const int IndexNumericLimit = 99;
        public const int AlignLengthLimit = 3;
        public const int AlignNumericLimit = 99;

        public const string PropertyNameValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
        public const string IndentStringValidChars = "0123456789";
        public const string AlignStringValidChars = "0123456789-";
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
