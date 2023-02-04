namespace OneI.Logable.Internal;

internal record struct TargetContext(
    InvocationExpressionSyntax SyntaxNode,
    IMethodSymbol Method)
{ }
