namespace OneI.Reflectable;

internal static class CodeAssets
{
    public const string TypeExtensions_ClassName = "OneTypeExtensions";
    public const string TypeExtensions_ClassFileName = $"{TypeExtensions_ClassName}.g.cs";
    public const string TypeExtensions_ClassConent = """
        // <auto-generated/>
        #nullable enable
        namespace OneI.Reflectable;

        using System;

        public static class OneTypeExtensions
        {
            public static Type GetOneType(this object? source)
            {
                if(source == null)
                {
                    throw new ArgumentNullException(nameof(source));
                }

                return null;
            }
        }
        """;

    public const string OneType_ClassName = "OneType";
    public const string OneType_ClassFileName = $"{OneType_ClassName}.g.cs";
    public const string OneType_ClassConent = """
        // <auto-generated/>
        #nullable enable
        namespace OneI.Reflectable;

        using System;

        public static class OneType
        {
            public static Type GetType<T>(T? source)
            {
                if(source == null)
                {
                    throw new ArgumentNullException(nameof(source));
                }

                return null;
            }
        }
        """;

    public const string OneReflection_ClassName = "OneReflectionAttribute";
    public const string OneReflection_ClassFileName = $"{OneReflection_ClassName}.g.cs";
    public const string OneReflection_ClassFullName = "OneI.Reflectable.OneReflectionAttribute";
    public const string OneReflection_ClassContent = """
        // <auto-generated/>
        namespace OneI.Reflectable;

        using System;

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
        public class OneReflectionAttribute : Attribute {}
        """;
}
