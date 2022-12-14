namespace OneI.Logable;

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using OneI.Logable.Definitions;

[Generator]
public class LoggerCodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterDefinitionFiles(context);

        var hasEnumSyntaxes = context.SyntaxProvider.CreateSyntaxProvider(
           static (s, token) => IsSyntaxTargetForGeneration(s, token),
           static (ctx, token) => BuildInvocationContext(ctx, token));

        var compilation = context.CompilationProvider
            .Combine(hasEnumSyntaxes.Collect());

        context.RegisterSourceOutput(compilation,
            static (spc, source)
            => Execute(source.Left, source.Right, spc));
    }

    /// <summary>
    /// 预先注册类文件
    /// </summary>
    /// <param name="context"></param>
    private static void RegisterDefinitionFiles(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(CodeAssets.LogFileName,
                SourceText.From(CodeAssets.LogFileContent, Encoding.UTF8));
        });

        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(CodeAssets.LoggerExtensionClassFileName,
                SourceText.From(CodeAssets.LoggerExtensionClassContent, Encoding.UTF8));
        });
    }

    /// <summary>
    /// 查找符合条件的节点
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        // 调用
        if(node is InvocationExpressionSyntax invocation)
        {
            var ma = invocation.Expression as MemberAccessExpressionSyntax;

            return ma?.RawKind == (int)SyntaxKind.SimpleMemberAccessExpression
                && invocation.ArgumentList.Arguments.Count > 0;
        }

        return false;
    }

    private static MethodDef? BuildInvocationContext(GeneratorSyntaxContext cts, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var invocation = (InvocationExpressionSyntax)cts.Node;

        var flag = false;
        var method = cts.SemanticModel.GetSymbolInfo((invocation.Expression as MemberAccessExpressionSyntax)!).Symbol as IMethodSymbol;

        if(method is { ContainingType: not null })
        {
            var isParams = method.Parameters.Any(x => x.IsParams);

            var typename = method.ContainingType.ToDisplayString();

            if(isParams
                && (typename == CodeAssets.LogClassFullName || typename == CodeAssets.LoggerExtensionFullName))
            {
                flag = true;
            }
        }

        if(!flag)
        {
            var errors = cts.SemanticModel.Compilation.GetDiagnostics()
                .Where(x => x.Severity == DiagnosticSeverity.Error);

            foreach(var item in errors)
            {
                Debug.WriteLine($"{item.GetMessage()} {item.Location}", nameof(LoggerCodeGenerator));
            }

            return null;
        }

        Debug.WriteLine(invocation.ToFullString(), nameof(LoggerCodeGenerator));

        return InvocationExpressionParser.TryParse(invocation, method!, cts);
    }

    private static void Execute(
        Compilation compilation,
        ImmutableArray<MethodDef?> methods,
        SourceProductionContext context)
    {
        if(methods.IsDefaultOrEmpty)
        {
            return;
        }

        CodePrinter.Print(methods, out var types, out var logExtensions, out var loggerExtensions);

        context.AddSource(CodeAssets.LoggerPropertyCreatorClassFileName, types);

        context.AddSource(CodeAssets.LogExtensionsFileName, logExtensions);

        context.AddSource(CodeAssets.LoggerExtensionExtensionClassFileName, loggerExtensions);
    }
}
