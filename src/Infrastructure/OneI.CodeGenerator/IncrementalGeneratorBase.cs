namespace OneI.CodeGenerator;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

/// <inheritdoc/>
public abstract class IncrementalGeneratorBase<TTargetSyntaxNode> : IIncrementalGenerator
{
    /// <inheritdoc/>
    public virtual void Initialize(IncrementalGeneratorInitializationContext context)
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
    protected virtual void RegisterPostInitializationOutput(IncrementalGeneratorPostInitializationContext context)
    {

    }

    /// <summary>
    /// 获取目标语法节点数据
    /// <para>或者使用<see cref="SyntaxValueProvider.ForAttributeWithMetadataName{T}(string, Func{SyntaxNode, CancellationToken, bool}, Func{GeneratorAttributeSyntaxContext, CancellationToken, T})"/>，已实现对特定<c>Attribute</c>的筛选</para>
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    protected virtual IncrementalValuesProvider<TTargetSyntaxNode?> GetTargetSyntaxNode(SyntaxValueProvider provider)
    {
        return provider.CreateSyntaxProvider(IsTargetSyntax, TransformTargetSyntaxNode);
    }

    /// <summary>
    /// 初步筛查符合条件的语法节点，之后进行转换
    /// </summary>
    /// <param name="node"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual bool IsTargetSyntax(SyntaxNode node, CancellationToken token)
    {
        return false;
    }

    /// <summary>
    /// 将符合条件的语法节点转换为目标节点
    /// </summary>
    /// <param name="cts"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual TTargetSyntaxNode? TransformTargetSyntaxNode(GeneratorSyntaxContext cts, CancellationToken token)
    {
        return default;
    }

    /// <summary>
    /// 根据给定的语法节点，生成源码
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="nodes"></param>
    /// <param name="context"></param>
    protected abstract void Execute(
        Compilation compilation,
        ImmutableArray<TTargetSyntaxNode?> nodes,
        SourceProductionContext context);
}
