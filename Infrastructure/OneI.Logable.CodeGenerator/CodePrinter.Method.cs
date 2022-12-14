namespace OneI.Logable;

using System.Text;
using OneI.Logable.Definitions;

/// <summary>
/// The code printer.
/// </summary>
public static partial class CodePrinter
{
    public static void PrintMethod(IndentedStringBuilder builder, MethodDef method)
    {
        builder.Append($"public static void {method.Name}");

        if(method.TypeArguments.Count > 0)
        {
            builder.Append($"<{string.Join(", ", method.TypeArguments.Keys)}>");
        }

        // 方法参数
        builder.Append("(");
        using(var _ = builder.Indent())
        {
            if(method.HasLevel)
            {
                builder.Append($"{CodeAssets.LogLevelParameterType} {CodeAssets.LogLevelParameterName}, ");
            }

            if(method.HasException)
            {
                builder.Append($"{CodeAssets.ExceptionParameterType} {CodeAssets.ExceptionParameterName}, ");
            }

            builder.Append($"{CodeAssets.MessageParameterType} {CodeAssets.MessageParameterName}, ");

            for(var i = 0; i < method.Parameters.Count; i++)
            {
                var item = method.Parameters[i];

                builder.Append($"{item.Type.ToDisplayString()} {item.Name}");

                if(i < method.Parameters.Count - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.AppendLine(")");

            // 泛型约束
            foreach(var x in method.TypeArguments
                .Where(x => string.IsNullOrWhiteSpace(x.Value) == false))
            {
                builder.AppendLine($"where {x.Key}: {x.Value}");
            }
        }

        builder.AppendLine("{");

        // body
        using(var _ = builder.Indent())
        {
            builder.AppendLine($"var propertyValues = new global::System.Collections.Generic.List<global::OneI.Logable.Templating.Properties.PropertyValue>({method.Parameters.Count});");

            builder.AppendLine();

            foreach(var parameter in method.Parameters)
            {
                var type = _types.FirstOrDefault(x => x.Equals(parameter.Type));
                if(type is not null)
                {
                    builder.AppendLine($"propertyValues.Add({CodeAssets.CreateMethodName}({parameter.Name}));");
                }
                else
                {
                    builder.AppendLine($"propertyValues.Add(new global::OneI.Logable.Templating.Properties.ValueTypes.LiteralValue<{parameter.Type}>({parameter.Name}));");
                }
            }

            builder.AppendLine();

            builder.Append($"WriteCore(");

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

            builder.AppendLine($"propertyValues);");
        }

        builder.AppendLine("}");
    }
}
