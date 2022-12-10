namespace OneI.Logable.Definitions;

using System.Collections.Concurrent;
using System.Collections.Generic;
using OneI.Logable.Infrastructure;

public class MethodDefinition
{
    public MethodDefinition(string name)
    {
        Name = name;
        Parameters = new();
        TypeArguments = new();
    }

    public string Name { get; }

    public Dictionary<string, string?> TypeArguments { get; }

    public bool HasLevel { get; private set; }

    public bool HasException { get; private set; }

    public List<ParameterDefinition> Parameters { get; }

    public void AddParameter(string type)
    {
        var parameter = new ParameterDefinition(Parameters.Count, type);

        Parameters.Add(parameter);
    }

    public void DefinedLevel() => HasLevel = true;

    public void DefinedException() => HasException = true;

    /// <summary>
    /// 添加类型参数
    /// </summary>
    /// <param name="type">类型参数名称</param>
    /// <param name="constraint">类型约束</param>
    public void AddTypeArgument(string type, string? constraint = null)
    {
        TypeArguments.Add(type, constraint);
    }

    public void AppendTo(IndentedStringBuilder builder, ConcurrentDictionary<string, TypeDefinition> _types)
    {
        builder.Append($"public static void {Name}");

        if(TypeArguments.Count > 0)
        {
            builder.Append($"<{string.Join(", ", TypeArguments.Keys)}>");
        }

        // 方法参数
        builder.Append("(");
        using(var _ = builder.Indent())
        {
            if(HasLevel)
            {
                builder.Append($"{CodeAssets.LogLevelParameterType} {CodeAssets.LogLevelParameterName}, ");
            }

            if(HasException)
            {
                builder.Append($"{CodeAssets.ExceptionParameterType} {CodeAssets.ExceptionParameterName}, ");
            }

            builder.Append($"{CodeAssets.MessageParameterType} {CodeAssets.MessageParameterName}, ");

            for(var i = 0; i < Parameters.Count; i++)
            {
                var item = Parameters[i];

                builder.Append($"{item.Type} {item.Name}");

                if(i < Parameters.Count - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.AppendLine(")");

            // 泛型约束
            foreach(var x in TypeArguments
                .Where(x => string.IsNullOrWhiteSpace(x.Value) == false))
            {
                builder.AppendLine($"where {x.Key}: {x.Value}");
            }
        }

        builder.AppendLine("{");

        // body
        using(var _ = builder.Indent())
        {
            builder.AppendLine($"var propertyValues = new global::System.Collections.Generic.List<global::OneI.Logable.Templating.Properties.PropertyValue>({Parameters.Count});");

            builder.AppendLine();

            foreach(var parameter in Parameters)
            {
                if(_types.TryGetValue(parameter.Type, out var type))
                {
                    switch(type.Kind)
                    {
                        case TypeKindEnum.ValueTuple:
                            break;
                        case TypeKindEnum.Nullable:
                            break;
                        case TypeKindEnum.Object:
                            break;
                        case TypeKindEnum.Enumerable:
                            break;
                        case TypeKindEnum.Dictionary:
                            break;
                        default:
                            builder.AppendLine($"propertyValues.Add({CodeAssets.CreatePropertyValueMethodName}({parameter.Name}));");
                            break;
                    }
                }
                else
                {
                    builder.AppendLine($"propertyValues.Add(new global::OneI.Logable.Templating.Properties.ValueTypes.LiteralValue<{parameter.Type}>({parameter.Name}));");
                }
            }

            builder.AppendLine();

            builder.Append($"WriteCore(");

            if(HasLevel)
            {
                builder.Append($"{CodeAssets.LogLevelParameterName}, ");
            }
            else
            {
                builder.Append($"{CodeAssets.LogLevelParameterType}.{Name}, ");
            }

            if(HasException)
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
