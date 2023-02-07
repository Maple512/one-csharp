namespace OneI.Generateable.EnumFastToString;

internal static class CodeAssets
{
    public static class Extension
    {
        public const string Namespace = "OneI.Generateable.CodeGenerated";

        public const string ClassName = "OneIGenerateableEnumExtensions";

        public const string FileName = $"{ClassName}.e.g.cs";

        public const string ToStringMethodName = "ToFastString";

        public const string DictionaryMethodName = "GetDictionaryMap";
    }

    public static class Attribute
    {
        public const string FullClassName = $"{Extension.Namespace}.ToFastStringAttribute";

        public const string FileName = "OneIGenerateableEnumExtensions.a.g.cs";

        public const string Content =
"""
#nullable enable
namespace OneI.Generateable.CodeGenerated
{
    /// <summary>
    /// Indicates to use the code generator to produce the ToFastString method for this Enum
    /// </summary>
    /// <remarks>
    /// <para>ex:</para>
    /// <code>
    /// public static string ToFastString(this SomeEnum @enum, [CallerArgumentExpression("enum")] string? expression = null)
    ///  => @enum switch
    ///  {
    ///      SomeEnum.A => nameof(SomeEnum.A),
    ///      SomeEnum.B => nameof(SomeEnum.B),
    ///      SomeEnum.C => nameof(SomeEnum.C),
    ///      _ => throw new ArgumentException($"The enum(\"{expression}\") does not in {nameof(SomeEnum)}."),
    ///  };
    /// </code>
    /// </remarks>
    [global::System.AttributeUsage(global::System.AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class ToFastStringAttribute : global::System.Attribute
    {
        /// <summary>
        /// Indicates to use the code generator to produce the ToFastString method for this Enum
        /// </summary>
        /// <remarks>
        /// <para>ex:</para>
        /// <code>
        /// public static string ToFastString(this SomeEnum @enum, [CallerArgumentExpression("enum")] string? expression = null)
        ///  => @enum switch
        ///  {
        ///      SomeEnum.A => nameof(SomeEnum.A),
        ///      SomeEnum.B => nameof(SomeEnum.B),
        ///      SomeEnum.C => nameof(SomeEnum.C),
        ///      _ => throw new ArgumentException($"The enum(\"{expression}\") does not in {nameof(SomeEnum)}."),
        ///  };
        /// </code>
        /// </remarks>
        public ToFastStringAttribute() { }

        /// <summary>
        /// Indicates to use the code generator to produce the ToFastString method for this Enum
        /// </summary>
        /// <param name="methodName">Custom extension method name, default: ToFastString</param>
        /// <remarks>
        /// <para>ex:</para>
        /// <code>
        /// public static string ToFastString(this SomeEnum @enum, [CallerArgumentExpression("enum")] string? expression = null)
        ///  => @enum switch
        ///  {
        ///      SomeEnum.A => nameof(SomeEnum.A),
        ///      SomeEnum.B => nameof(SomeEnum.B),
        ///      SomeEnum.C => nameof(SomeEnum.C),
        ///      _ => throw new ArgumentException($"The enum(\"{expression}\") does not in {nameof(SomeEnum)}."),
        ///  };
        /// </code>
        /// </remarks>
        public ToFastStringAttribute(global::System.String? methodName)
        {
            MethodName = methodName;
        }

        /// <summary>
        /// Custome ToFastString method name.
        /// </summary>
        public global::System.String? MethodName { get; }

        /// <summary>
        /// Generate enum and number mapping dictionary
        /// </summary>
        public global::System.Boolean HasDictionary { get; init; }

        /// <summary>
        /// Mapping dictionary method name
        /// </summary>
        public global::System.String? DictionaryMethodName { get; init; }
    }
}
#nullable restore
""";
    }
}
