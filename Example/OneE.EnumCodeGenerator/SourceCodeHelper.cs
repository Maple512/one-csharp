namespace OneE.EnumCodeGenerator;

using System.Collections.Generic;
using System.Text;

public static class SourceCodeHelper
{
    public const string AttributeFileName = "EnumExtensionsAttribute.g.cs";

    public const string AttributeNamespaceName = "OneE.EnumCodeGenerator";

    public const string AttributeClassName = "EnumExtensionsAttribute";

    public const string AttributeFullName = $"{AttributeNamespaceName}.{AttributeClassName}";

    public const string Attribute = @"
namespace OneE.EnumCodeGenerator
{
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    public class EnumExtensionsAttribute : System.Attribute
    {
    }
}";

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
