namespace OneI.Logable;

using OneI.Logable.Definitions;
using OneI.Logable.Internal;

partial class CodePrinter
{
    internal static void PrintType(IndentedStringBuilder builder, TypeDef type)
    {
        var wrapperName = $"{string.Join("_", type.Names)}_Wrapper";

        builder.AppendLine($"private class {wrapperName} : global::OneI.Logable.Templates.IPropertyValueFormattable");

        builder.AppendLine("{");

        using(builder.Indent())
        {
            builder.AppendLine($"{type.ToDisplayString()} value;");

            builder.AppendLine($"public {wrapperName}({type.ToDisplayString()} arg)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("value = arg;");
            }
            builder.AppendLine("}");

            builder.AppendLine();
            builder.AppendLine($"public global::System.Boolean TryFormat(global::System.Span<char> destination, global::OneI.Logable.Templates.PropertyType type, out global::System.Int32 charsWritten)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("charsWritten = 0;");

                builder.AppendLine("var container = new global::OneI.Text.RefValueStringBuilder(destination);");
                builder.AppendLine("container.Append('{');");
                var index = 0;
                foreach(var item in type.Properties)
                {
                    builder.AppendLine($"container.Append(\"{item.Name}: \");");
                    if(item.Type.IsSpanFormattable)
                    {
                        builder.AppendLine($"container.AppendSpanFormattable(value.{item.Name});");
                    }
                    else if(item.Type.IsFormattable)
                    {
                        builder.AppendLine($"container.AppendSpanFormattable(value.{item.Name});");
                    }

                    if(++index < type.Properties.Count)
                    {
                        builder.AppendLine("container.Append(',');");
                    }
                }
                builder.AppendLine("container.Append('}');");
                builder.AppendLine("container.Dispose();");

                builder.AppendLine("return true;");
            }
            builder.AppendLine("}");
        }

        builder.AppendLine("}");

        //if(type.Kind is not TypeDefKind.Array
        //                and not TypeDefKind.Enumerable
        //                and not TypeDefKind.Dictionary)
        //{
        //    builder.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        //}

        //builder.AppendLine($"public static global::OneI.Logable.Templates.PropertyValue Create_{string.Join("_", type.Names)}({type.ToDisplayString()} arg)");

        //builder.AppendLine("{");

        //using(var _ = builder.Indent())
        //{
        //    switch(type.Kind)
        //    {
        //        case TypeDefKind.Object:
        //            BuildObject(builder, type);
        //            break;

        //        case TypeDefKind.ValueTuple:
        //            BuildValueTuple(builder, type);
        //            break;

        //        case TypeDefKind.Nullable:
        //            BuildNullable(builder, type);
        //            break;

        //        case TypeDefKind.Array:
        //        case TypeDefKind.Enumerable:
        //            BuildEnumerable(builder);
        //            break;

        //        case TypeDefKind.Dictionary:
        //            BuildDictionary(builder, type);
        //            break;

        //        default:
        //            Default(builder, type);
        //            break;
        //    }
        //}

        //builder.AppendLine("}");
    }

    private static void Default(IndentedStringBuilder builder, TypeDef type)
        => builder.AppendLine("return new global::OneI.Logable.Templates.PropertyValue(arg);");

    private static void BuildEnumerable(IndentedStringBuilder builder)
    {
        builder.AppendLine("var array = new global::OneI.Logable.Templates.EnumerableValue();");
        builder.AppendLine();
        builder.AppendLine("foreach(var item in arg)");
        builder.AppendLine("{");
        using(var _ = builder.Indent())
        {
            builder.AppendLine("array.Add(Create(item));");
        }

        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine("return array;");
    }

    private static void BuildObject(IndentedStringBuilder builder, TypeDef type)
    {
        if(type.Kind is not TypeDefKind.Object)
        {
            return;
        }

        builder.AppendLine("var value = new global::OneI.Logable.Templates.ObjectValue();");
        builder.AppendLine();

        for(var i = 0; i < type.Properties.Count; i++)
        {
            var item = type.Properties[i];

            builder.AppendLine($"value.Add(\"{item.Name}\", Create(arg.{item.Name}));");
        }

        builder.AppendLine();
        builder.AppendLine("return value;");
    }

    private static void BuildValueTuple(IndentedStringBuilder builder, TypeDef type)
    {
        if(type.Kind is not TypeDefKind.ValueTuple)
        {
            return;
        }

        builder.AppendLine("var value = new global::OneI.Logable.Templates.ObjectValue();");
        builder.AppendLine();

        for(var i = 0; i < type.Properties.Count; i++)
        {
            var item = type.Properties[i];

            builder.AppendLine($"value.AddProperty(\"{item.Name}\", Create(arg.Item{i + 1}));");
        }

        builder.AppendLine();
        builder.AppendLine("return value;");
    }

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
        builder.AppendLine("return Create(new global::OneI.Logable.Templates.LiteralValue<object>(null));");
    }

    public static void BuildDictionary(IndentedStringBuilder builder, TypeDef type)
    {
        builder.AppendLine("var dictionary = new global::OneI.Logable.Templates.DictionaryValue();");
        builder.AppendLine();
        builder.AppendLine("foreach(var item in arg)");
        builder.AppendLine("{");
        using(var _ = builder.Indent())
        {
            builder.AppendLine("dictionary.Add(Create(item.Key), Create(item.Value));");
        }

        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine("return dictionary;");
    }
}
