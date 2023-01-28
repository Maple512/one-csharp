namespace OneI.Logable;

using Definitions;
using Microsoft.CodeAnalysis.Text;

internal static partial class CodePrinter
{
    private static readonly HashSet<TypeDef> _types = new();

    public static void AddType(TypeDef type)
    {
        _types.Add(type);
    }

    internal static void Print(IEnumerable<MethodDef> methods, out SourceText tyeps, out SourceText logExtensions, out SourceText loggerExtensions)
    {
        tyeps = PrintTypes();

        loggerExtensions = PrintLoggerExtensions(methods.Where(x => x.IsLogger).ToList());

        logExtensions = PrintLogExtensions(methods.Where(x => x.IsLogger == false).ToList());
    }

    private static SourceText PrintTypes()
    {
        var content = new IndentedStringBuilder();

        content.AppendLine($"// <auto-generated/> {DateTime.Now:HH:mm:ss}");
        content.AppendLine("#nullable enable");
        content.AppendLine("namespace OneI.Logable;");
        content.AppendLine();
        content.AppendLine("using global::System;");
        content.AppendLine("using global::System.Runtime.CompilerServices;");
        content.AppendLine();
        content.AppendLine("[global::System.Diagnostics.DebuggerStepThrough]");
        content.AppendLine($"internal static class {CodeAssets.LoggerPropertyCreatorClassName}");
        content.AppendLine("{");

        using(var _ = content.Indent())
        {
            foreach(var item in _types)
            {
                PrintType(content, item);

                content.AppendLine();
            }
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
        content.AppendLine("namespace OneI.Logable;");
        content.AppendLine();
        content.AppendLine("using global::System;");
        content.AppendLine();
        content.AppendLine($"public static partial class {CodeAssets.LoggerExtensionClassName}");
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
        content.AppendLine("#nullable restore");

        return SourceText.From(content.ToString(), Encoding.UTF8);
    }

    private static SourceText PrintLogExtensions(List<MethodDef> methods)
    {
        var content = new IndentedStringBuilder();

        content.AppendLine($"// <auto-generated/> {DateTime.Now:HH:mm:ss}");
        content.AppendLine("#nullable enable");
        content.AppendLine("namespace OneI.Logable;");
        content.AppendLine();
        content.AppendLine("using global::System;");
        content.AppendLine();
        content.AppendLine("public static partial class Log");
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
        content.AppendLine("#nullable restore");

        return SourceText.From(content.ToString(), Encoding.UTF8);
    }
}
