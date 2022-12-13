namespace OneI.Logable;
using System.Text;
using OneI.Logable.Definitions;

/// <summary>
/// The code printer.
/// </summary>
public static partial class CodePrinter
{
    public static void PrintType(IndentedStringBuilder builder, TypeDef type)
    {
        builder.AppendLine($"private static global::OneI.Logable.Templating.Properties.PropertyValue {CodeAssets.CreateMethodName}({type.ToDisplayString()} p)");

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

    private static void Default(IndentedStringBuilder builder, TypeDef type)
    {
        builder.AppendLine($"return new global::OneI.Logable.Templating.Properties.ValueTypes.LiteralValue<{type.ToDisplayString()}>(p);");
    }

    private static void BuildEnumerable(IndentedStringBuilder builder)
    {
        builder.AppendLine($"var array = new global::OneI.Logable.Templating.Properties.ValueTypes.EnumerableValue();");
        builder.AppendLine();
        builder.AppendLine($"foreach(var item in p)");
        builder.AppendLine("{");
        using(var _ = builder.Indent())
        {
            builder.AppendLine($"array.AddPropertyValue(Create(item));");
        }

        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine($"return array;");
    }

    private static void BuildObject(IndentedStringBuilder builder, TypeDef type)
    {
        if(type.Kind is not TypeDefKind.Object)
        {
            return;
        }

        builder.AppendLine($"var valueTuple = new global::OneI.Logable.Templating.Properties.ValueTypes.ObjectValue();");
        builder.AppendLine();

        for(var i = 0; i < type.Properties.Count; i++)
        {
            var item = type.Properties[i];

            builder.AppendLine($"valueTuple.AddProperty(\"{item.Name}\", Create(p.{item.Name}));");
        }

        builder.AppendLine();
        builder.AppendLine($"return valueTuple;");
    }

    private static void BuildValueTuple(IndentedStringBuilder builder, TypeDef type)
    {
        if(type.Kind is not TypeDefKind.ValueTuple)
        {
            return;
        }

        builder.AppendLine($"var valueTuple = new global::OneI.Logable.Templating.Properties.ValueTypes.ObjectValue();");
        builder.AppendLine();

        for(var i = 0; i < type.Properties.Count; i++)
        {
            var item = type.Properties[i];

            builder.AppendLine($"valueTuple.AddProperty(\"{item.Name}\", Create(p.Item{i + 1}));");
        }

        builder.AppendLine();
        builder.AppendLine($"return valueTuple;");
    }

    public static void BuildNullable(IndentedStringBuilder builder, TypeDef type)
    {
        builder.AppendLine("if(p.HasValue)");
        builder.AppendLine("{");
        using(var _ = builder.Indent())
        {
            builder.AppendLine("return Create(p.Value);");
        }
        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine("return Create(new global::OneI.Logable.Templating.Properties.ValueTypes.LiteralValue<object>(null));");
    }

    public static void BuildDictionary(IndentedStringBuilder builder, TypeDef type)
    {
        builder.AppendLine($"var dictionary = new global::OneI.Logable.Templating.Properties.ValueTypes.DictionaryValue();");
        builder.AppendLine();
        builder.AppendLine($"foreach(var item in p)");
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
