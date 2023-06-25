namespace OneI.Logable.Internal;

using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;
using OneI.Generateable.Generator;

internal static partial class CodePrinter
{
    internal static void Print(IEnumerable<MethodDef> methods, out SourceText tyeps, out SourceText loggerExtensions)
    {
        tyeps = PrintTypes();

        loggerExtensions = PrintLoggerExtensions(methods.ToList());
    }

    private static SourceText PrintTypes()
    {
        var content = new IndentedStringBuilder();

        content.AppendLine($"// <auto-generated/> {DateTime.Now:HH:mm:ss}");
        content.AppendLine("#nullable enable");
        content.AppendLine($"namespace {CodeAssets.LoggerExtension.Namespace}");
        content.AppendLine("{");
        using(content.Indent())
        {
            content.AppendLine("using global::System;");
            content.AppendLine("using global::System.Runtime.CompilerServices;");
            content.AppendLine();
            content.AppendLine($"internal static partial class {CodeAssets.LoggerExtension.Name}");
            content.AppendLine("{");

            // 创建自定义属性
            using(var _ = content.Indent())
            {
                var types = TypeCache.Types.OrderBy(x => x.Kind)
                    .ThenBy(x => x.WrapperName)
                    .ThenBy(x => x.Properties.Count)
                    .ToList();

                if(types.FirstOrDefault(x => x.Equals(TypeSymbolParser.DefaultType)) == null)
                {
                    types.Add(TypeSymbolParser.DefaultType);
                }

                var index = 0;
                foreach(var item in types)
                {
                    PrintType(content, item);

                    if(++index < types.Count)
                    {
                        content.AppendLine();
                    }
                }

                var formatter = types.Where(
                    x => x.Kind is not TypeDefKind.Literal or TypeDefKind.None)
                    .ToList();
                if(formatter.Count > 0)
                {
                    content.AppendLine();
                    index = 0;
                    foreach(var item in formatter)
                    {
                        switch(item.Kind)
                        {
                            case TypeDefKind.Tuple:
                            case TypeDefKind.ValueTuple:
                                TupleFormatter(content, item);
                                break;
                            case TypeDefKind.Array:
                                ArrayFormatter(content, item);
                                break;
                            case TypeDefKind.EnumerableT:
                                EnumerableTFormatter(content, item);
                                break;
                            case TypeDefKind.Dictionary:
                                DictionaryFormatter(content, item);
                                break;
                            case TypeDefKind.Enumerable:
                                EnumerableFormatter(content, item);
                                break;
                            case TypeDefKind.Object:
                                ObjectTypeFormatter(content, item);
                                break;
                        }

                        if(++index < formatter.Count)
                        {
                            content.AppendLine();
                        }
                    }
                }
            }

            content.AppendLine("}");
        }

        content.AppendLine("}");

        content.AppendLine("#nullable restore");

        return SourceText.From(content.ToString(), Encoding.UTF8);
    }

    private static SourceText PrintLoggerExtensions(List<MethodDef> methods)
    {
        var content = new IndentedStringBuilder();

        content.AppendLine($"// <auto-generated/> {DateTime.Now:HH:mm:ss}");
        content.AppendLine("#nullable enable");
        content.AppendLine($"namespace {CodeAssets.LoggerExtension.Namespace}");
        content.AppendLine("{");
        using(content.Indent())
        {
            content.AppendLine("using global::System;");
            content.AppendLine("using global::System.Runtime.CompilerServices;");
            content.AppendLine();
            content.AppendLine($"internal static partial class {CodeAssets.LoggerExtension.Name}");
            content.AppendLine("{");

            using(var _ = content.Indent())
            {
                var count = methods.Count;

                for(var i = 0; i < count; i++)
                {
                    var item = methods[i];

                    PrintMethod(content, item!);

                    if(i < count - 1)
                    {
                        content.AppendLine();
                    }
                }
            }

            content.AppendLine("}");
        }

        content.AppendLine("}");

        content.AppendLine("#nullable restore");

        return SourceText.From(content.ToString(), Encoding.UTF8);
    }
}
