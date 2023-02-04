namespace OneI.Logable;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;
using OneI.Logable.Definitions;
using OneI.Logable.Internal;

[Generator]
public class LoggerCodeGenerator : IIncrementalGenerator
{
    private static readonly string[] _methodNames = "Write,Verbose,Debug,Information,Warning,Error,Fatal,".Split(',');

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
    ///     注册初始化之后的输出
    /// </summary>
    /// <param name="context"></param>
    private static void RegisterPostInitializationOutput(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(CodeAssets.LoggerExtension.FileNmae, SourceText.From(CodeAssets.LoggerExtension.Content, Encoding.UTF8));
    }

    /// <summary>
    ///     获取目标语法节点数据
    ///     <para>
    ///         或者使用
    ///         <see
    ///             cref="SyntaxValueProvider.ForAttributeWithMetadataName{T}(string, Func{SyntaxNode, CancellationToken, bool}, Func{GeneratorAttributeSyntaxContext, CancellationToken, T})" />
    ///         ，已实现对特定<c>Attribute</c>的筛选
    ///     </para>
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    private static IncrementalValuesProvider<TargetContext?> GetTargetSyntaxNode(SyntaxValueProvider provider)
        => provider.CreateSyntaxProvider(IsTargetSyntax, TransformTargetSyntaxNode);

    /// <summary>
    ///     初步筛查符合条件的语法节点，之后进行转换
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

            if(ma?.RawKind != (int)SyntaxKind.SimpleMemberAccessExpression)
            {
                return false;
            }

            var count = invocation.ArgumentList.Arguments.Count;

            return count is > 0 and < 15;
        }

        return false;
    }

    /// <summary>
    ///     将符合条件的语法节点转换为目标节点
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

        if(symbolInfo.Symbol is not IMethodSymbol method)
        {
            return null;
        }

        if(!_methodNames.Contains(method.Name))
        {
            return null;
        }

        var typename = method.ContainingType?.ToDisplayString();

        if(typename is not CodeAssets.LoggerExtension.ClassFullName)
        {
            return null;
        }

        return new TargetContext(invocation, method);
    }

    /// <summary>
    ///     根据给定的语法节点，生成源码
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

        var methods = new Dictionary<int, MethodDef>();
        foreach(var item in nodes.Where(x => x is not null).Select(x => x!.Value))
        {
            if(InvocationExpressionParser.TryParse(item!, compilation, out var result))
            {
                var key = result!.GetHashCode();

                if(methods.ContainsKey(key) == false)
                {
                    methods.Add(key, result);
                }
            }
        }

        if(methods.Any())
        {
            CodePrinter.Print(methods.Values, out var types, out var loggerExtensions);

            context.AddSource(CodeAssets.PropertyFactory.FileName, types);

            context.AddSource(CodeAssets.LoggerExtension.PartialName, loggerExtensions);
        }
    }
}

