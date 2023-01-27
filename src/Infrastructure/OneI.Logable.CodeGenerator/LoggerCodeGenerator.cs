namespace OneI.Logable;

using System.Collections.Immutable;
using System.Linq;
using Definitions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

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

    private static readonly string[] _methodNames = "Write,Verbose,Debug,Information,Warning,Error,Fatal,".Split(',');

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

        var memberAccessSyntax = (invocation.Expression as MemberAccessExpressionSyntax)!;

        var symbolInfo = cts.SemanticModel.GetSymbolInfo(memberAccessSyntax);

        var method = symbolInfo.Symbol as IMethodSymbol;

        if(method is not null)
        {
            var isParams = method.Parameters.Any(x => x.IsParams);
            var name = method.Name;
            if(isParams == false || !_methodNames.Contains(name))
            {
                return null;
            }

            var typename = method.ContainingType?.ToDisplayString();

            if(typename is CodeAssets.LogClassFullName)
            {
                return new(invocation, false, method.Name, method.Parameters);
            }

            if(typename is CodeAssets.LoggerExtensionFullName)
            {
                return new(invocation, true, method.Name, method.Parameters);
            }

            return null;
        }

        method = symbolInfo.CandidateSymbols.FirstOrDefault() as IMethodSymbol;
        if(method is not null)
        {
            var typename = method.ContainingType?.ToDisplayString();

            if(typename is CodeAssets.LoggerFullName)
            {
                return new(invocation, true, method.Name, method.Parameters.Take(method.Parameters.Length - 3).ToImmutableArray());
            }

            return null;
        }

        return null;
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

        var methods = new List<MethodDef>();
        foreach(var item in nodes.Where(x => x is not null).Select(x => x!.Value))
        {
            if(InvocationExpressionParser.TryParse(item!, compilation, out var result))
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
}
