namespace OneI.Logable.Definitions;

using System.Collections.Concurrent;
using System.Collections.Generic;
using OneI.Logable.Infrastructure;

public class MethodDefinition
{
    public MethodDefinition()
    {
        Parameters = new();
        VariableParameters = new();
        TypeArguments = new();
        GenericConstraints = new();
    }

    public List<string> TypeArguments { get; }

    public Dictionary<string, string?> GenericConstraints { get; }

    public ParameterDefinition? Message { get; private set; }

    public ParameterDefinition? Level { get; private set; }

    public ParameterDefinition? Exception { get; private set; }

    public List<ParameterDefinition> Parameters { get; }

    public List<ParameterDefinition> VariableParameters { get; }

    public void AddParameter(string type, bool isException = false, bool isTypeParameter = false)
    {
        var parameter = new ParameterDefinition(Parameters.Count, type, isException, isTypeParameter);

        Parameters.Add(parameter);

        switch(parameter.Kind)
        {
            case ParameterKind.LogLevel:
                if(Level is null)
                {
                    Level = parameter;
                    return;
                }

                break;
            case ParameterKind.Message:
                if(Message is null)
                {
                    Message = parameter;
                    return;
                }

                break;
            case ParameterKind.Exception:
                if(Exception is null)
                {
                    Exception = parameter;
                    return;
                }

                break;
        }

        VariableParameters.Add(parameter);
    }

    /// <summary>
    /// 添加类型参数
    /// </summary>
    /// <param name="type">类型参数名称</param>
    /// <param name="constraint">类型约束</param>
    public void AddTypeArgument(string type, string? constraint = null)
    {
        TypeArguments.Add(type);

        if(constraint is not null and { Length: > 0 })
        {
            GenericConstraints.Add(type, constraint);
        }
    }

    public void AppendTo(IndentedStringBuilder builder, ConcurrentDictionary<string, TypeDefinition> _types)
    {
        builder.Append($"public static void Write");

        if(TypeArguments.Count > 0)
        {
            builder.Append($"<{string.Join(", ", TypeArguments)}>");
        }

        // arguments type & name
        builder.AppendLine("(");
        using(var _ = builder.Indent())
        {
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
            foreach(var x in GenericConstraints
                .Where(x => string.IsNullOrWhiteSpace(x.Value) == false))
            {
                builder.AppendLine($"where {x.Key}: {x.Value}");
            }
        }

        builder.AppendLine("{");

        // body
        using(var _ = builder.Indent())
        {
            builder.AppendLine($"var propertyValues = new global::System.Collections.Generic.List<global::OneI.Logable.Templating.Properties.PropertyValue>({VariableParameters.Count});");

            foreach(var parameter in VariableParameters)
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
                            builder.AppendLine($"propertyValues.Add(new global::OneI.Logable.Templating.Properties.ValueTypes.LiteralValue<{type}>({parameter.Name}));");
                            break;
                    }
                }
                else
                {
                    builder.AppendLine($"propertyValues.Add(new global::OneI.Logable.Templating.Properties.ValueTypes.LiteralValue<{parameter.Type}>({parameter.Name}));");
                }
            }

            builder.AppendLine();

            builder.AppendLine($"WarpMessage({Level!.Name}, {Message!.Name}, null, propertyValues);");
        }

        builder.AppendLine("}");
    }
}
