namespace OneI.Logable;

using System.Text;
using OneI.Logable.Definitions;

/// <summary>
/// The code printer.
/// </summary>
internal static partial class CodePrinter
{
    /// <summary>
    /// Prints the method.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="method">The method.</param>
    internal static void PrintMethod(IndentedStringBuilder builder, MethodDef method)
    {
        builder.Append($"public static void {method.Name}");

        if(method.TypeArguments.Count > 0)
        {
            builder.Append($"<{string.Join(", ", method.TypeArguments.Keys)}>");
        }

        // 方法参数
        builder.AppendLine("(");
        using(var _ = builder.Indent())
        {
            PrintMethodParameters(builder, method);
        }

        builder.AppendLine("{");

        // body
        using(var _ = builder.Indent())
        {
            PrintMethodBody(builder, method);
        }

        builder.AppendLine("}");
    }

    /// <summary>
    /// Prints the method parameters.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="method">The method.</param>
    private static void PrintMethodParameters(IndentedStringBuilder builder, MethodDef method)
    {
        if(method.IsLogger)
        {
            builder.AppendLine($"this global::{CodeAssets.LoggerFullName} logger, ");
        }

        if(method.HasLevel)
        {
            builder.AppendLine($"{CodeAssets.LogLevelParameterType} {CodeAssets.LogLevelParameterName}, ");
        }

        if(method.HasException)
        {
            builder.AppendLine($"{CodeAssets.ExceptionParameterType} {CodeAssets.ExceptionParameterName}, ");
        }

        builder.AppendLine($"{CodeAssets.MessageParameterType} {CodeAssets.MessageParameterName}, ");

        for(var i = 0; i < method.Parameters.Count; i++)
        {
            var item = method.Parameters[i];

            builder.AppendLine($"{item.Type.ToDisplayString()} {item.Name}, ");
        }

        builder.AppendLine("[global::System.Runtime.CompilerServices.CallerFilePath] global::System.String? filePath = null, ");
        builder.AppendLine("[global::System.Runtime.CompilerServices.CallerMemberName] global::System.String? memberName = null, ");
        builder.AppendLine("[global::System.Runtime.CompilerServices.CallerLineNumber] global::System.Int32? lineNumber = null)");

        // 泛型约束
        foreach(var x in method.TypeArguments
            .Where(x => string.IsNullOrWhiteSpace(x.Value) == false))
        {
            builder.AppendLine($"where {x.Key}: {x.Value}");
        }
    }

    /// <summary>
    /// Prints the method body.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="method">The method.</param>
    private static void PrintMethodBody(IndentedStringBuilder builder, MethodDef method)
    {
        builder.AppendLine($"var propertyValues = new global::System.Collections.Generic.List<global::OneI.Textable.Templating.Properties.PropertyValue>({method.Parameters.Count});");

        builder.AppendLine();

        foreach(var parameter in method.Parameters)
        {
            var type = _types.FirstOrDefault(x => x.Equals(parameter.Type));
            if(type is not null)
            {
                builder.AppendLine($"propertyValues.Add({CodeAssets.LoggerPropertyCreateCalledName}({parameter.Name}));");
            }
            else
            {
                builder.AppendLine($"propertyValues.Add(new global::OneI.Textable.Templating.Properties.LiteralValue<{parameter.Type}>({parameter.Name}));");
            }
        }

        builder.AppendLine();

        // call LoggerExtensions.Write
        if(method.IsLogger)
        {
            builder.Append($"global::OneI.Logable.LoggerExtensions.PackageWrite(logger, ");
        }
        else
        {
            builder.Append($"global::OneI.Logable.LoggerExtensions.PackageWrite(_logger, ");
        }

        if(method.HasLevel)
        {
            builder.Append($"{CodeAssets.LogLevelParameterName}, ");
        }
        else
        {
            builder.Append($"{CodeAssets.LogLevelParameterType}.{method.Name}, ");
        }

        if(method.HasException)
        {
            builder.Append($"{CodeAssets.ExceptionParameterName}, ");
        }
        else
        {
            builder.Append("null, ");
        }

        builder.Append($"{CodeAssets.MessageParameterName}, ");

        builder.AppendLine($"propertyValues, filePath, memberName, lineNumber);");
    }
}
