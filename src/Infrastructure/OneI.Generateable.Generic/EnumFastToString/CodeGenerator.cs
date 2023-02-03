namespace OneI.Generateable.EnumFastToString;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class CodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(RegisterPostInitializationOutput);

        var target = GetTargetSyntaxNode(context.SyntaxProvider);

        var compilation = context.CompilationProvider
                                 .Combine(target.Collect());

        context.RegisterSourceOutput(compilation,
                                     (spc, source)
                                         => Execute(source.Left, source.Right, spc));

    }

    private static IncrementalValuesProvider<EnumDeclarationInformation?> GetTargetSyntaxNode(SyntaxValueProvider provider)
        => provider.CreateSyntaxProvider(IsTargetSyntax, TransformTargetSyntaxNode);

    private static bool IsTargetSyntax(SyntaxNode node, CancellationToken token)
    {
        return node is EnumDeclarationSyntax and { AttributeLists.Count: > 0 };
    }

    private static EnumDeclarationInformation? TransformTargetSyntaxNode(GeneratorSyntaxContext cts, CancellationToken token)
    {
        var enumSyntax = (EnumDeclarationSyntax)cts.Node;

        foreach(var attributeListSyntax in enumSyntax.AttributeLists)
        {
            foreach(var attributeSyntax in attributeListSyntax.Attributes)
            {
                var symbolInfo = cts.SemanticModel.GetSymbolInfo(attributeSyntax);
                if(symbolInfo.Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingType = attributeSymbol.ContainingType;

                var fullname = attributeContainingType.ToDisplayString();

                if(fullname == CodeAssets.Attribute.FullClassName)
                {
                    var methodName = attributeSyntax.ArgumentList?.Arguments.FirstOrDefault()?.ToFullString()
                        ?.Replace("\"", null)
                        ?? CodeAssets.Extension.MethodDefaultName;

                    return (enumSyntax, methodName);
                }
            }
        }

        return null;
    }

    private static void Execute(Compilation compilation
                                , ImmutableArray<EnumDeclarationInformation?> nodes
                                , SourceProductionContext context)
    {
        if(nodes.IsDefaultOrEmpty)
        {
            return;
        }

        var enums = Parse(compilation, nodes.Where(x => x is not null)
            .Select(x => x.Value).Distinct()!, context.CancellationToken);

        context.AddSource(CodeAssets.Extension.FileName, SourceText.From(Build(enums), Encoding.UTF8));
    }

    private static List<EnumInformation> Parse(
        Compilation compilation,
        IEnumerable<EnumDeclarationInformation> syntaxes,
        CancellationToken cancellationToken)
    {
        var result = new List<EnumInformation>();

        var attribute = compilation.GetTypeByMetadataName(CodeAssets.Attribute.FullClassName);

        if(attribute == null)
        {
            return result;
        }

        foreach((var syntax, var methodName) in syntaxes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var symbol = compilation.GetSemanticModel(syntax.SyntaxTree)
                .GetDeclaredSymbol(syntax, cancellationToken);

            if(symbol is not INamedTypeSymbol named)
            {
                continue;
            }

            var name = symbol.ToString();
            var fields = symbol.GetMembers()
                .Where(x => x is IFieldSymbol { ConstantValue: { } })
                .Select(x => x.Name)
                .ToList();

            result.Add(new EnumInformation(name, methodName, fields));
        }

        return result;
    }

    private static string Build(List<EnumInformation> enums)
    {
        var container = new IndentedStringBuilder();

        container.AppendLine($"namespace {CodeAssets.Extension.Namespace}");
        container.AppendLine("{");
        using(container.Indent())
        {
            container.AppendLine($"public static partial class {CodeAssets.Extension.ClassName}");
            container.AppendLine("{");
            using(container.Indent())
            {
                foreach(var item in enums)
                {
                    var name = $"global::{item.Name}";
                    container.AppendLine($"public static string {item.MethodName}(this {name} @enum, [CallerArgumentExpression(\"enum\")] string? expression = null)");
                    using(container.Indent())
                    {
                        container.AppendLine("=> @enum switch");
                        container.AppendLine("{");
                        using(container.Indent())
                        {
                            foreach(var field in item.Fields)
                            {
                                container.AppendLine($"{name}.{field} => nameof({name}.{field}),");
                            }

                            container.AppendLine($"_ => throw new ArgumentException($\"The enum(\\\"{{expression}}\\\") does not in \\\"{item.Name}\\\".\"),");
                        }

                        container.AppendLine("};");
                    }
                }
            }

            container.AppendLine("}");
        }

        container.AppendLine("}");

        return container.ToString();
    }

    /// <summary>
    /// 注册初始定义文件
    /// </summary>
    /// <param name="context"></param>
    private static void RegisterPostInitializationOutput(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(CodeAssets.Attribute.FileName, SourceText.From(CodeAssets.Attribute.Content, Encoding.UTF8));
    }
}

internal record struct EnumInformation(string Name, string? MethodName, List<string> Fields)
{
}

internal record struct EnumDeclarationInformation(EnumDeclarationSyntax Syntax, string MethodName)
{
    public static implicit operator (EnumDeclarationSyntax Syntax, string MethodName)(EnumDeclarationInformation value)
    {
        return (value.Syntax, value.MethodName);
    }

    public static implicit operator EnumDeclarationInformation((EnumDeclarationSyntax Syntax, string MethodName) value)
    {
        return new EnumDeclarationInformation(value.Syntax, value.MethodName);
    }
}
