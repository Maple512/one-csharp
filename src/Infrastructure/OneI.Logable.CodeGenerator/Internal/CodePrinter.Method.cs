namespace OneI.Logable;

using OneI.Logable.Definitions;
using OneI.Logable.Internal;

internal partial class CodePrinter
{
    internal static void PrintMethod(IndentedStringBuilder builder, MethodDef method)
    {
        builder.Append($"public static void {method.Name}");

        if(method.TypeArguments.Count > 0)
        {
            builder.Append($"<{string.Join(", ", method.TypeArguments.Keys)}>");
        }

        // 方法参数
        builder.AppendLine("(");
        using(builder.Indent())
        {
            PrintMethodParameters(builder, method);
        }

        builder.AppendLine("{");

        // body
        using(builder.Indent())
        {
            PrintMethodBody(builder, method);
        }

        builder.AppendLine("}");
    }

    private static void PrintMethodParameters(IndentedStringBuilder builder, MethodDef method)
    {
        builder.AppendLine($"this global::{CodeAssets.LoggerFullName} logger, ");

        if(method.HasLevel)
        {
            builder.AppendLine($"{CodeAssets.LogLevelParameterType} {CodeAssets.LogLevelParameterName}, ");
        }

        if(method.HasException)
        {
            builder.AppendLine($"{CodeAssets.ExceptionParameterType} {CodeAssets.ExceptionParameterName}, ");
        }

        if(method.HasMessage)
        {
            builder.AppendLine($"{CodeAssets.MessageParameterType} {CodeAssets.MessageParameterName}, ");
        }

        for(var i = 0; i < method.Parameters.Count; i++)
        {
            var item = method.Parameters[i];

            builder.AppendLine($"{item.Type.ToDisplayString()} {item.Name}, ");
        }

        builder.AppendLine("[global::System.Runtime.CompilerServices.CallerFilePath] global::System.String? file = null, ");
        builder.AppendLine("[global::System.Runtime.CompilerServices.CallerMemberName] global::System.String? member = null, ");
        builder.AppendLine("[global::System.Runtime.CompilerServices.CallerLineNumber] global::System.Int32 line = 0)");

        // 泛型约束
        foreach(var x in method.TypeArguments
                               .Where(x => string.IsNullOrWhiteSpace(x.Value) == false))
        {
            builder.AppendLine($"where {x.Key}: {x.Value}");
        }
    }

    private static void PrintMethodBody(IndentedStringBuilder builder, MethodDef method)
    {
        builder.AppendLine("var properties = new global::OneI.Logable.Templates.PropertyDictionary();");

        for(var i = 0; i < method.Parameters.Count; i++)
        {
            var parameter = method.Parameters[i];

            builder.Append($"properties.Add(\"__{i}\", ");

            var type = _types.Values.FirstOrDefault(x => x.Equals(parameter.Type));

            if(type.Kind is not TypeDefKind.Literal)
            {
                builder.Append($"new global::OneI.Logable.Templates.PropertyValue(new {string.Join("_", type.Names)}_Wrapper({parameter.Name}))");
            }
            else
            {
                builder.Append($"new global::OneI.Logable.Templates.PropertyValue({parameter.Name})");
            }

            builder.AppendLine(");");
        }

        builder.AppendLine();

        builder.Append("global::OneI.Logable.LoggerExtensions.WriteCore(logger, ");

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

        if(method.HasMessage)
        {
            builder.Append($"{CodeAssets.MessageParameterName}, ");
        }
        else
        {
            builder.Append($"null, ");
        }

        builder.AppendLine("ref properties, file, member, line);");

        builder.AppendLine();

        builder.AppendLine("properties.Dispose();");
    }
}
