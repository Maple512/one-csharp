namespace OneI.Logable;

using System.Collections.Immutable;

internal record struct TargetContext(InvocationExpressionSyntax SyntaxNode, bool IsLogger, string Name, ImmutableArray<IParameterSymbol> Parameters)
{
}
