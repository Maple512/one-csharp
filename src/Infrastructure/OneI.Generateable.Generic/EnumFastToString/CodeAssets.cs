namespace OneI.Generateable.EnumFastToString;

internal static class CodeAssets
{
    public static class Extension
    {
        public const string Namespace = "OneI.Generateable";

        public const string ClassName = "OneIGenerateableEnumExtensions";

        public const string FileName = $"{ClassName}.e.g.cs";

        public const string MethodDefaultName = "ToFastString";
    }

    public static class Attribute
    {
        public const string FullClassName = "OneI.Generateable.ToFastStringAttribute";

        public const string FileName = "OneIGenerateableEnumExtensions.a.g.cs";

        public const string Content =
"""
namespace OneI.Generateable
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
    [global::System.AttributeUsage(System.AttributeTargets.Enum)]
    internal class ToFastStringAttribute : global::System.Attribute
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
        public ToFastStringAttribute(string methodName) 
        {
            MethodName = methodName;
        }

        public string? MethodName { get; }
    }
}
""";
    }
}
