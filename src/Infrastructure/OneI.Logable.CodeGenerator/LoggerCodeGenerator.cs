namespace OneI.Logable;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;
using OneI.Logable.Definitions;

/// <inheritdoc/>
[Generator]
public class LoggerCodeGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
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

    /// <summary>
    /// 注册初始化之后的输出
    /// </summary>
    /// <param name="context"></param>
    private static void RegisterPostInitializationOutput(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(CodeAssets.LogFileName,
                 SourceText.From(CodeAssets.LogFileContent, Encoding.UTF8));

        context.AddSource(CodeAssets.LoggerExtensionClassFileName,
            SourceText.From(CodeAssets.LoggerExtensionClassContent, Encoding.UTF8));
    }

    /// <summary>
    /// 获取目标语法节点数据
    /// <para>或者使用<see cref="SyntaxValueProvider.ForAttributeWithMetadataName{T}(string, Func{SyntaxNode, CancellationToken, bool}, Func{GeneratorAttributeSyntaxContext, CancellationToken, T})"/>，已实现对特定<c>Attribute</c>的筛选</para>
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    private static IncrementalValuesProvider<TargetContext?> GetTargetSyntaxNode(SyntaxValueProvider provider)
    {
        return provider.CreateSyntaxProvider(IsTargetSyntax, TransformTargetSyntaxNode);
    }

    /// <summary>
    /// 初步筛查符合条件的语法节点，之后进行转换
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private static bool IsTargetSyntax(SyntaxNode node, CancellationToken token)
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

    /// <summary>
    /// 将符合条件的语法节点转换为目标节点
    /// </summary>
    /// <param name="cts"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private static TargetContext? TransformTargetSyntaxNode(GeneratorSyntaxContext cts, CancellationToken token)
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

        return new(invocation, method!);
    }

    /// <summary>
    /// 根据给定的语法节点，生成源码
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="nodes"></param>
    /// <param name="context"></param>
    private static void Execute(
        Compilation compilation,
        ImmutableArray<TargetContext?> nodes,
        SourceProductionContext context)
    {
        if(nodes.IsDefaultOrEmpty)
        {
            return;
        }

        var targetNodes = nodes.Where(x => x is not null)!;
        var methods = new List<MethodDef>();
        foreach(var item in targetNodes)
        {
            if(InvocationExpressionParser.TryParse(item!.Value.SyntaxNode, item!.Value.MethodSymbol!, compilation, out var result))
            {
                methods.Add(result!);
            }
        }

        if(methods.Any())
        {
            CodePrinter.Print(methods, out var types, out var logExtensions, out var loggerExtensions);

            context.AddSource(CodeAssets.LoggerPropertyCreatorClassFileName, types);

            context.AddSource(CodeAssets.LogExtensionsFileName, logExtensions);

            context.AddSource(CodeAssets.LoggerExtensionExtensionClassFileName, loggerExtensions);
        }
    }

    record struct TargetContext(InvocationExpressionSyntax SyntaxNode, IMethodSymbol MethodSymbol)
    {
    }
}
