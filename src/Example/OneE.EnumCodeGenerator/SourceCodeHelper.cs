namespace OneE.EnumCodeGenerator;

/// <summary>
/// The source code helper.
/// </summary>

public static class SourceCodeHelper
{
    /// <summary>
    /// The attribute file name.
    /// </summary>
    public const string AttributeFileName = "EnumExtensionsAttribute.g.cs";

    /// <summary>
    /// The attribute namespace name.
    /// </summary>
    public const string AttributeNamespaceName = "OneE.EnumCodeGenerator";

    /// <summary>
    /// The attribute class name.
    /// </summary>
    public const string AttributeClassName = "EnumExtensionsAttribute";

    /// <summary>
    /// The attribute full name.
    /// </summary>
    public const string AttributeFullName = $"{AttributeNamespaceName}.{AttributeClassName}";

    /// <summary>
    /// The attribute.
    /// </summary>
    public const string Attribute = """

        namespace OneE.EnumCodeGenerator
        {
            [System.AttributeUsage(System.AttributeTargets.Enum)]
            public class EnumExtensionsAttribute : System.Attribute
            {
            }
        }
        """;

    /// <summary>
    /// Generates the extension class.
    /// </summary>
    /// <param name="enumsToGenerate">The enums to generate.</param>
    /// <returns>A string.</returns>
    public static string GenerateExtensionClass(List<EnumToGenerate> enumsToGenerate)
    {
        var sb = new StringBuilder();
        sb.Append(@"
namespace OneE.EnumCodeGenerator
{
    public static partial class EnumExtensions
    {");
        foreach(var enumToGenerate in enumsToGenerate)
        {
            sb.Append(@"
                public static string ToStringFast(this ").Append(enumToGenerate.Name).Append(@" value)
                    => value switch
                    {");
            foreach(var member in enumToGenerate.Values)
            {
                sb.Append(@"
                ").Append(enumToGenerate.Name).Append('.').Append(member)
                    .Append(" => nameof(")
                    .Append(enumToGenerate.Name).Append('.').Append(member).Append("),");
            }

            sb.Append(@"
                    _ => value.ToString(),
                };
");
        }

        sb.Append(@"
    }
}");

        return sb.ToString();
    }
}
