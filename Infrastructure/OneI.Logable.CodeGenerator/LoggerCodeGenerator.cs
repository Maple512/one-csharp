namespace OneI.Logable;

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using OneI.Logable.Definitions;
using OneI.Logable.Infrastructure;
using OneI.Logable.Parsing;

[Generator]
public class LoggerCodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterDefinitionFiles(context);

        var hasEnumSyntaxes = context.SyntaxProvider.CreateSyntaxProvider(
           static (s, _) => IsSyntaxTargetForGeneration(s),
           static (ctx, _) => BuildInvocationContext(ctx));

        var compilation = context.CompilationProvider
            .Combine(hasEnumSyntaxes.Collect());

        context.RegisterSourceOutput(compilation,
            static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    /// <summary>
    /// 预先注册类文件
    /// </summary>
    /// <param name="context"></param>
    private static void RegisterDefinitionFiles(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(
                CodeAssets.LogClassFileName,
                SourceText.From(CodeAssets.LogClassContent, Encoding.UTF8));
        });
    }

    /// <summary>
    /// 查找符合条件的节点
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        // 调用
        if(node is InvocationExpressionSyntax invocation)
        {
            var ma = invocation.Expression as MemberAccessExpressionSyntax;

            return ma?.RawKind == (int)SyntaxKind.SimpleMemberAccessExpression
                && invocation.ArgumentList.Arguments.Count > 0;
        }

        return false;
    }

    private static MethodDefinition? BuildInvocationContext(GeneratorSyntaxContext cts)
    {
        var invocation = (InvocationExpressionSyntax)cts.Node;

        Debug.WriteLine(invocation.ToFullString(), nameof(LoggerCodeGenerator));

        var typeConfimded = false;
        if(cts.SemanticModel.GetSymbolInfo((invocation.Expression as MemberAccessExpressionSyntax)!).Symbol
            is IMethodSymbol method
            && method is { ContainingType: not null })
        {
            var typename = method.ContainingType.ToDisplayString();

            if(typename == CodeAssets.LogClassFullName)
            {
                typeConfimded = true;
            }
        }

        if(!typeConfimded)
        {
            var diagnostics = cts.SemanticModel.Compilation.GetDiagnostics();

            foreach(var item in diagnostics)
            {
                Debug.WriteLine(item.GetMessage(), nameof(LoggerCodeGenerator));
            }

            return null;
        }

        return InvocationParser.TryParser(invocation, cts);
    }

    private static void Execute(
        Compilation compilation,
        ImmutableArray<MethodDefinition?> methods,
        SourceProductionContext context)
    {
        if(methods.IsDefaultOrEmpty)
        {
            return;
        }

        var content = new IndentedStringBuilder();

        InvocationParser.Wrap(content, methods);

        context.AddSource(CodeAssets.LogImplClassFileName, SourceText.From(content.ToString(), Encoding.UTF8));
    }
}
