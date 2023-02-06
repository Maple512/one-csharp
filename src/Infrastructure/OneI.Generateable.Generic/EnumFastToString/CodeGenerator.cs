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
                if(symbolInfo.Symbol is not IMethodSymbol method)
                {
                    continue;
                }

                var type = method.ContainingType;

                var fullname = type.ToDisplayString();

                if(fullname == CodeAssets.Attribute.FullClassName)
                {
                    var baseType = "global::System.Int32";

                    if(enumSyntax.BaseList is { Types: { Count: > 0 } })
                    {
                        var typeSymbol = cts.SemanticModel.GetSymbolInfo(enumSyntax.BaseList.Types.First().Type).Symbol;
                        if(typeSymbol is not null)
                        {
                            if(TypeSymbolParser.TryParse(typeSymbol, out var btype))
                            {
                                baseType = btype!.ToDisplayString();
                            }
                        }
                    }

                    var methodName = CodeAssets.Extension.ToStringMethodName;
                    var hasDictionary = false;
                    string? dictionaryMethodName = null;

                    if(attributeSyntax.ArgumentList is { Arguments: { Count: > 0 } })
                    {
                        for(var i = 0; i < attributeSyntax.ArgumentList!.Arguments.Count; i++)
                        {
                            var argument = attributeSyntax.ArgumentList!.Arguments[i];
                            var argumentName = argument.NameEquals?.Name.ToFullString().Trim();

                            var methodParameter = method.Parameters.FirstOrDefault();
                            var expression = argument.Expression as LiteralExpressionSyntax;

                            if(methodParameter?.Name == "methodName")
                            {
                                methodName = argument.Expression
                                    .ToFullString()
                                    .Replace("\"", null) ?? CodeAssets.Extension.ToStringMethodName;
                            }
                            else if(argumentName == "DictionaryMethodName")
                            {
                                dictionaryMethodName = argument.Expression
                                    .ToFullString()
                                    .Replace("\"", null);
                            }
                            else if(argumentName == "HasDictionary")
                            {
                                hasDictionary = expression?.IsKind(SyntaxKind.TrueLiteralExpression) == true;
                            }
                        }
                    }

                    if(hasDictionary)
                    {
                        dictionaryMethodName ??= CodeAssets.Extension.DictionaryMethodName;
                    }
                    else if(dictionaryMethodName is { Length: > 0 })
                    {
                        hasDictionary = true;
                    }

                    return (enumSyntax, methodName, hasDictionary, dictionaryMethodName, baseType);
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
            .Select(x => x!.Value).Distinct()!, context.CancellationToken)
            .OrderBy(x => x.Name);

        context.AddSource(CodeAssets.Extension.FileName, SourceText.From(Build(enums.ToList()), Encoding.UTF8));
    }

    private static IEnumerable<EnumInformation> Parse(
        Compilation compilation,
        IEnumerable<EnumDeclarationInformation> syntaxes,
        CancellationToken cancellationToken)
    {
        var attribute = compilation.GetTypeByMetadataName(CodeAssets.Attribute.FullClassName);

        if(attribute == null)
        {
            return Array.Empty<EnumInformation>();
        }

        var result = new List<EnumInformation>();

        foreach(var @enum in syntaxes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var symbol = compilation.GetSemanticModel(@enum.Syntax.SyntaxTree)
                .GetDeclaredSymbol(@enum.Syntax, cancellationToken);

            if(symbol is not INamedTypeSymbol named)
            {
                continue;
            }

            var name = symbol.ToString();
            var fields = symbol.GetMembers().OfType<IFieldSymbol>()
                .Where(x => x is IFieldSymbol)
                .ToDictionary(k => k.Name, v => v.ConstantValue);

            result.Add(new EnumInformation(name, @enum, fields));
        }

        return result;
    }

    private static string Build(List<EnumInformation> enums)
    {
        var container = new IndentedStringBuilder();

        container.AppendLine("#nullable enable");
        container.AppendLine($"namespace {CodeAssets.Extension.Namespace}");
        container.AppendLine("{");
        using(container.Indent())
        {
            container.AppendLine($"public static partial class {CodeAssets.Extension.ClassName}");
            container.AppendLine("{");
            using(container.Indent())
            {
                var index = 0;
                foreach(var item in enums)
                {
                    var name = $"global::{item.Name}";
                    container.AppendLine($"public static string {item.Declaration.MethodName}(this {name} @enum, [CallerArgumentExpression(nameof(@enum))] string? expression = null)");
                    using(container.Indent())
                    {
                        container.AppendLine("=> @enum switch");
                        container.AppendLine("{");
                        using(container.Indent())
                        {
                            foreach(var field in item.Fields)
                            {
                                container.AppendLine($"{name}.{field.Key} => nameof({name}.{field.Key}),");
                            }

                            container.AppendLine($"_ => throw new ArgumentException($\"The enum(\\\"{{expression}}\\\") does not in \\\"{item.Name}\\\".\"),");
                        }

                        container.AppendLine("};");
                    }

                    if(item.Declaration.HasDictionary)
                    {
                        container.AppendLine();
                        var returnType = $"global::System.Collections.Generic.Dictionary<{item.Declaration.BaseType}, global::System.String>";

                        container.AppendLine($"public static {returnType} {item.Declaration.DictionaryMethodName}(this {name} @enum)");
                        using(container.Indent())
                        {
                            container.AppendLine($"=> new {returnType}({item.Fields.Count})");
                            container.AppendLine("{");
                            using(container.Indent())
                            {
                                var findex = 0;
                                foreach(var field in item.Fields)
                                {
                                    container.Append($"{{({item.Declaration.BaseType}){field.Value}, nameof({name}.{field.Key})}}");

                                    if(++findex < item.Fields.Count)
                                    {
                                        container.AppendLine(",");
                                    }
                                    else
                                    {
                                        container.AppendLine(string.Empty);
                                    }
                                }
                            }

                            container.AppendLine("};");
                        }
                    }

                    if(++index < enums.Count)
                    {
                        container.AppendLine();
                    }
                }
            }

            container.AppendLine("}");
        }

        container.AppendLine("}");
        container.AppendLine("#nullable restore");

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

internal record struct EnumInformation(string Name, EnumDeclarationInformation Declaration, Dictionary<string, object?> Fields)
{
}

internal record struct EnumDeclarationInformation(
    EnumDeclarationSyntax Syntax,
    string MethodName,
    bool HasDictionary,
    string? DictionaryMethodName,
    string BaseType)
{
    public static implicit operator (EnumDeclarationSyntax Syntax, string MethodName, bool HasDictionary, string? DictionaryMethodName, string BaseType)(EnumDeclarationInformation value)
    {
        return (value.Syntax, value.MethodName, value.HasDictionary, value.DictionaryMethodName, value.BaseType);
    }

    public static implicit operator EnumDeclarationInformation((EnumDeclarationSyntax Syntax, string MethodName, bool HasDictionary, string? DictionaryMethodName, string BaseType) value)
    {
        return new EnumDeclarationInformation(value.Syntax, value.MethodName, value.HasDictionary, value.DictionaryMethodName, value.BaseType);
    }
}
