namespace OneI.Reflectable;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class TypeCodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterDefinitionFiles(context);

        var target = context.SyntaxProvider.CreateSyntaxProvider(
           static (s, token) => IsTargetSyntax(s, token),
           static (ctx, token) => BuildTargetSyntaxContext(ctx, token));

        var compilation = context.CompilationProvider
            .Combine(target.Collect());

        context.RegisterSourceOutput(compilation,
            static (spc, source)
            => Execute(source.Left, source.Right, spc));
    }

    /// <summary>
    /// 初步查找目标语法
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private static bool IsTargetSyntax(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return node is MemberDeclarationSyntax syntax and { AttributeLists.Count: > 0 };
    }

    /// <summary>
    /// 进一步确定目标语法，并构建目标语法上下文
    /// </summary>
    /// <param name="cts"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private static MemberDeclarationSyntax? BuildTargetSyntaxContext(GeneratorSyntaxContext cts, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var member = (MemberDeclarationSyntax)cts.Node;

        foreach(var attributeList in member.AttributeLists)
        {
            foreach(var attribute in attributeList.Attributes)
            {
                if(cts.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var type = attributeSymbol.ContainingType;

                var fullname = type.ToDisplayString();

                if(fullname == CodeAssets.OneReflection_ClassFullName)
                {
                    return member;
                }
            }
        }

        return null;
    }

    private static void Execute(
        Compilation compilation,
        ImmutableArray<MemberDeclarationSyntax?> methods,
        SourceProductionContext context)
    {
        if(methods.IsDefaultOrEmpty)
        {
            return;
        }

        //CodePrinter.Print(methods, out var types, out var logExtensions, out var loggerExtensions);

        //context.AddSource(CodeAssets.LoggerPropertyCreatorClassFileName, types);

        //context.AddSource(CodeAssets.LogExtensionsFileName, logExtensions);

        //context.AddSource(CodeAssets.LoggerExtensionExtensionClassFileName, loggerExtensions);
    }

    /// <summary>
    /// 预先注册扩展类
    /// </summary>
    /// <param name="context"></param>
    private static void RegisterDefinitionFiles(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(CodeAssets.TypeExtensions_ClassFileName,
                SourceText.From(CodeAssets.TypeExtensions_ClassConent, Encoding.UTF8));

            ctx.AddSource(CodeAssets.OneType_ClassFileName,
                SourceText.From(CodeAssets.OneType_ClassConent, Encoding.UTF8));

            ctx.AddSource(CodeAssets.OneReflection_ClassFileName,
                SourceText.From(CodeAssets.OneReflection_ClassContent, Encoding.UTF8));
        });
    }
}
