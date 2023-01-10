namespace OneI.Logable;

using System.Text;
using OneI.Logable.Definitions;

internal static partial class CodePrinter
{
    internal static void PrintType(IndentedStringBuilder builder, TypeDef type)
    {
        builder.AppendLine($"public static global::OneI.Textable.Templating.Properties.PropertyValue {CodeAssets.LoggerPropertyCreateMethodName}({type.ToDisplayString()} arg)");

        builder.AppendLine("{");

        using(var _ = builder.Indent())
        {
            switch(type.Kind)
            {
                case TypeDefKind.Object:
                    BuildObject(builder, type);
                    break;

                case TypeDefKind.ValueTuple:
                    BuildValueTuple(builder, type);
                    break;

                case TypeDefKind.Nullable:
                    BuildNullable(builder, type);
                    break;

                case TypeDefKind.Array:
                case TypeDefKind.Enumerable:
                    BuildEnumerable(builder);
                    break;

                case TypeDefKind.Dictionary:
                    BuildDictionary(builder, type);
                    break;

                default:
                    Default(builder, type);
                    break;
            }
        }

        builder.AppendLine("}");
    }

    /// <summary>
    /// Defaults the.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="type">The type.</param>
    private static void Default(IndentedStringBuilder builder, TypeDef type)
    {
        builder.AppendLine($"return new global::OneI.Textable.Templating.Properties.LiteralValue<{type.ToDisplayString()}>(arg);");
    }

    /// <summary>
    /// Builds the enumerable.
    /// </summary>
    /// <param name="builder">The builder.</param>
    private static void BuildEnumerable(IndentedStringBuilder builder)
    {
        builder.AppendLine($"var array = new global::OneI.Textable.Templating.Properties.EnumerableValue();");
        builder.AppendLine();
        builder.AppendLine($"foreach(var item in arg)");
        builder.AppendLine("{");
        using(var _ = builder.Indent())
        {
            builder.AppendLine($"array.Add(Create(item));");
        }

        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine($"return array;");
    }

    /// <summary>
    /// Builds the object.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="type">The type.</param>
    private static void BuildObject(IndentedStringBuilder builder, TypeDef type)
    {
        if(type.Kind is not TypeDefKind.Object)
        {
            return;
        }

        builder.AppendLine($"var valueTuple = new global::OneI.Textable.Templating.Properties.ObjectValue();");
        builder.AppendLine();

        for(var i = 0; i < type.Properties.Count; i++)
        {
            var item = type.Properties[i];

            builder.AppendLine($"valueTuple.Add(\"{item.Name}\", Create(arg.{item.Name}));");
        }

        builder.AppendLine();
        builder.AppendLine($"return valueTuple;");
    }

    /// <summary>
    /// Builds the value tuple.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="type">The type.</param>
    private static void BuildValueTuple(IndentedStringBuilder builder, TypeDef type)
    {
        if(type.Kind is not TypeDefKind.ValueTuple)
        {
            return;
        }

        builder.AppendLine($"var valueTuple = new global::OneI.Textable.Templating.Properties.ObjectValue();");
        builder.AppendLine();

        for(var i = 0; i < type.Properties.Count; i++)
        {
            var item = type.Properties[i];

            builder.AppendLine($"valueTuple.AddProperty(\"{item.Name}\", Create(arg.Item{i + 1}));");
        }

        builder.AppendLine();
        builder.AppendLine($"return valueTuple;");
    }

    /// <summary>
    /// Builds the nullable.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="type">The type.</param>
    public static void BuildNullable(IndentedStringBuilder builder, TypeDef type)
    {
        builder.AppendLine("if(arg.HasValue)");
        builder.AppendLine("{");
        using(var _ = builder.Indent())
        {
            builder.AppendLine("return Create(arg.Value);");
        }

        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine("return Create(new global::OneI.Textable.Templating.Properties.LiteralValue<object>(null));");
    }

    /// <summary>
    /// Builds the dictionary.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="type">The type.</param>
    public static void BuildDictionary(IndentedStringBuilder builder, TypeDef type)
    {
        builder.AppendLine($"var dictionary = new global::OneI.Textable.Templating.Properties.DictionaryValue();");
        builder.AppendLine();
        builder.AppendLine($"foreach(var item in arg)");
        builder.AppendLine("{");
        using(var _ = builder.Indent())
        {
            builder.AppendLine($"dictionary.Add(Create(item.Key), Create(item.Value));");
        }

        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine($"return dictionary;");
    }
}
