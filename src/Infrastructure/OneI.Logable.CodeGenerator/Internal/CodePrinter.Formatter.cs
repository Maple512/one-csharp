namespace OneI.Logable.Internal;

using OneI.Logable.Definitions;

internal partial class CodePrinter
{
    private static void ObjectTypeFormatter(IndentedStringBuilder builder, TypeDef type)
    {
        builder.AppendLine($"private class {type.FormatterName} : global::OneI.Logable.Templates.IPropertyValueFormattable");
        builder.AppendLine("{");
        using(builder.Indent())
        {
            builder.AppendLine($"{type.ToDisplayString()} value;");
            builder.AppendLine($"public {type.FormatterName}({type.ToDisplayString()} arg)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("value = arg;");
            }

            builder.AppendLine("}");
            builder.AppendLine();
            builder.AppendLine($"public void Format(ref global::Cysharp.Text.Utf16ValueStringBuilder container, global::OneI.Logable.Templates.PropertyType type)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("container.Append('{');");
                var index = 0;
                foreach(var item in type.Properties)
                {
                    var property = $"value.{item.Name}";

                    builder.AppendLine($"global::OneI.Logable.Templates.TemplateRenderHelper.LiteralRender(ref container, {item.Type.WrapperMethod}({property}), type, null, null);");

                    if(++index < type.Properties.Count)
                    {
                        builder.AppendLine("container.Append(\", \");");
                    }
                }

                builder.AppendLine("container.Append('}');");
            }

            builder.AppendLine("}");
        }

        builder.AppendLine("}");
    }

    private static void TupleFormatter(IndentedStringBuilder builder, TypeDef type)
    {
        builder.AppendLine($"private class {type.FormatterName} : global::OneI.Logable.Templates.IPropertyValueFormattable");

        builder.AppendLine("{");

        using(builder.Indent())
        {
            builder.AppendLine($"{type.ToDisplayString()} obj;");

            builder.AppendLine($"public {type.FormatterName}({type.ToDisplayString()} value)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("obj = value;");
            }

            builder.AppendLine("}");
            builder.AppendLine();
            builder.AppendLine($"public void Format(ref global::Cysharp.Text.Utf16ValueStringBuilder container, global::OneI.Logable.Templates.PropertyType type)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("container.Append('{');");
                var index = 1;
                foreach(var item in type.Properties)
                {
                    var property = $"obj.Item{index}";

                    builder.AppendLine($"global::OneI.Logable.Templates.TemplateRenderHelper.LiteralRender(ref container, {item.Type.WrapperMethod}({property}), type, null, null);");

                    if(index++ < type.Properties.Count)
                    {
                        builder.AppendLine("container.Append(\", \");");
                    }
                }

                builder.AppendLine("container.Append('}');");
            }

            builder.AppendLine("}");
        }

        builder.AppendLine("}");
    }

    private static void EnumerableTFormatter(IndentedStringBuilder builder, TypeDef typeDef)
    {
        var type = typeDef.TypeArguments[0];
        var elementType = type.ToDisplayString();

        builder.AppendLine($"class {typeDef.FormatterName} : global::OneI.Logable.Templates.IPropertyValueFormattable");
        builder.AppendLine("{");
        using(builder.Indent())
        {
            builder.AppendLine($"global::System.Collections.Generic.IEnumerable<{elementType}> enumerable;");
            builder.AppendLine($"public {typeDef.FormatterName}(global::System.Collections.Generic.IEnumerable<{elementType}> enumerable)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("this.enumerable = enumerable;");
            }

            builder.AppendLine();
            builder.AppendLine("}");
            builder.AppendLine();
            builder.AppendLine("public void Format(ref global::Cysharp.Text.Utf16ValueStringBuilder container, global::OneI.Logable.Templates.PropertyType type)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("var enumerator = enumerable.GetEnumerator();");
                builder.AppendLine("var hasNext = false;");
                builder.AppendLine();
                builder.AppendLine("container.Append('[');");
                builder.AppendLine("while(true)");
                builder.AppendLine("{");
                using(builder.Indent())
                {
                    builder.AppendLine("if(hasNext || enumerator.MoveNext())");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine($"var wrapper = {type.WrapperMethod}(enumerator.Current);");

                        builder.AppendLine($"global::OneI.Logable.Templates.TemplateRenderHelper.LiteralRender(ref container, wrapper, type, null, null);");
                    }

                    builder.AppendLine("}");
                    builder.AppendLine("else");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine("break;");
                    }

                    builder.AppendLine("}");
                    builder.AppendLine("if(enumerator.MoveNext())");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine("hasNext = true;");
                        builder.AppendLine("container.Append(\", \");");
                    }

                    builder.AppendLine("}");
                    builder.AppendLine("else");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine("hasNext = false;");
                    }

                    builder.AppendLine("}");
                }

                builder.AppendLine("}");
                builder.AppendLine("container.Append(']');");

            }

            builder.AppendLine("}");
        }

        builder.AppendLine("}");
    }

    private static void EnumerableFormatter(IndentedStringBuilder builder, TypeDef typeDef)
    {
        var type = TypeSymbolParser.DefaultType;

        builder.AppendLine($"class {typeDef.FormatterName} : global::OneI.Logable.Templates.IPropertyValueFormattable");
        builder.AppendLine("{");
        using(builder.Indent())
        {
            builder.AppendLine($"global::System.Collections.IEnumerable enumerable;");
            builder.AppendLine($"public {typeDef.FormatterName}(global::System.Collections.IEnumerable enumerable)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("this.enumerable = enumerable;");
            }

            builder.AppendLine("}");
            builder.AppendLine();
            builder.AppendLine("public void Format(ref global::Cysharp.Text.Utf16ValueStringBuilder container, global::OneI.Logable.Templates.PropertyType type)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("var enumerator = enumerable.GetEnumerator();");
                builder.AppendLine("var hasNext = false;");
                builder.AppendLine();
                builder.AppendLine("container.Append('[');");
                builder.AppendLine("while(true)");
                builder.AppendLine("{");
                using(builder.Indent())
                {
                    builder.AppendLine("if(hasNext || enumerator.MoveNext())");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine($"var wrapper = {type.WrapperMethod}(enumerator.Current);");

                        builder.AppendLine($"global::OneI.Logable.Templates.TemplateRenderHelper.LiteralRender(ref container, wrapper, type, null, null);");
                    }

                    builder.AppendLine("}");
                    builder.AppendLine("else");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine("break;");
                    }

                    builder.AppendLine("}");
                    builder.AppendLine("if(enumerator.MoveNext())");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine("hasNext = true;");
                        builder.AppendLine("container.Append(\", \");");
                    }

                    builder.AppendLine("}");
                    builder.AppendLine("else");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine("hasNext = false;");
                    }

                    builder.AppendLine("}");
                }

                builder.AppendLine("}");
                builder.AppendLine("container.Append(']');");

            }

            builder.AppendLine("}");
        }

        builder.AppendLine("}");
    }

    private static void DictionaryFormatter(IndentedStringBuilder builder, TypeDef typeDef)
    {
        var key = typeDef.TypeArguments[0];
        var value = typeDef.TypeArguments[1];
        var elementType = $"{key.ToDisplayString()},{value.ToDisplayString()}";

        builder.AppendLine($"class {typeDef.FormatterName} : global::OneI.Logable.Templates.IPropertyValueFormattable");
        builder.AppendLine("{");
        using(builder.Indent())
        {
            builder.AppendLine($"global::System.Collections.Generic.Dictionary<{elementType}> enumerable;");
            builder.AppendLine($"public {typeDef.FormatterName}(global::System.Collections.Generic.Dictionary<{elementType}> enumerable)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("this.enumerable = enumerable;");
            }

            builder.AppendLine("}");
            builder.AppendLine();
            builder.AppendLine("public void Format(ref global::Cysharp.Text.Utf16ValueStringBuilder container, global::OneI.Logable.Templates.PropertyType type)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("var enumerator = enumerable.GetEnumerator();");
                builder.AppendLine("var hasNext = false;");
                builder.AppendLine();
                builder.AppendLine("container.Append('[');");
                builder.AppendLine("while(true)");
                builder.AppendLine("{");
                using(builder.Indent())
                {
                    builder.AppendLine("if(hasNext || enumerator.MoveNext())");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine("container.Append('{');");
                        builder.AppendLine($"var keyWrapper = {key.WrapperMethod}(enumerator.Current.Key);");
                        builder.AppendLine($"global::OneI.Logable.Templates.TemplateRenderHelper.LiteralRender(ref container, keyWrapper, type, null, null);");
                        builder.AppendLine("container.Append(':');");
                        builder.AppendLine($"var valueWrapper = {value.WrapperMethod}(enumerator.Current.Value);");
                        builder.AppendLine($"global::OneI.Logable.Templates.TemplateRenderHelper.LiteralRender(ref container, valueWrapper, type, null, null);");
                        builder.AppendLine("container.Append('}');");
                    }

                    builder.AppendLine("}");
                    builder.AppendLine("else");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine("break;");
                    }

                    builder.AppendLine("}");
                    builder.AppendLine("if(enumerator.MoveNext())");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine("hasNext = true;");
                        builder.AppendLine("container.Append(\", \");");
                    }

                    builder.AppendLine("}");
                    builder.AppendLine("else");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine("hasNext = false;");
                    }

                    builder.AppendLine("}");
                }

                builder.AppendLine("}");
                builder.AppendLine("container.Append(']');");

            }

            builder.AppendLine("}");
        }

        builder.AppendLine("}");
    }

    private static void ArrayFormatter(IndentedStringBuilder builder, TypeDef typeDef)
    {
        var type = typeDef.TypeArguments[0];
        var elementType = type.ToDisplayString();

        builder.AppendLine($"class {typeDef.FormatterName} : global::OneI.Logable.Templates.IPropertyValueFormattable");
        builder.AppendLine("{");
        using(builder.Indent())
        {
            builder.AppendLine($"{elementType}[] enumerable;");
            builder.AppendLine($"public {typeDef.FormatterName}({elementType}[] enumerable)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("this.enumerable = enumerable;");
            }

            builder.AppendLine("}");
            builder.AppendLine();
            builder.AppendLine("public void Format(ref global::Cysharp.Text.Utf16ValueStringBuilder container, global::OneI.Logable.Templates.PropertyType type)");
            builder.AppendLine("{");
            using(builder.Indent())
            {
                builder.AppendLine("container.Append('[');");
                builder.AppendLine("for (int i = 0; i < enumerable.Length; i++)");
                builder.AppendLine("{");
                using(builder.Indent())
                {
                    builder.AppendLine($"var wrapper = {type.WrapperMethod}(enumerable[i]);");

                    builder.AppendLine($"global::OneI.Logable.Templates.TemplateRenderHelper.LiteralRender(ref container, wrapper, type, null, null);");

                    builder.AppendLine("if(i < enumerable.Length - 1)");
                    builder.AppendLine("{");
                    using(builder.Indent())
                    {
                        builder.AppendLine("container.Append(\", \");");
                    }

                    builder.AppendLine("}");
                }

                builder.AppendLine("}");
                builder.AppendLine("container.Append(']');");
            }

            builder.AppendLine("}");
        }

        builder.AppendLine("}");
    }
}
