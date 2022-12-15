namespace OneI.Logable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OneI.Logable.Definitions;

public static class InvocationExpressionParser
{
    public static MethodDef? TryParse(InvocationExpressionSyntax invocation, IMethodSymbol method, GeneratorSyntaxContext cts)
    {
        try
        {
            return Parse(invocation, method, cts);
        }
        catch(Exception)
        {
            throw;
        }
    }

    private static MethodDef Parse(InvocationExpressionSyntax invocation, IMethodSymbol methodSymbol, GeneratorSyntaxContext cts)
    {
        var method = new MethodDef(methodSymbol.Name)
        {
            IsLogger = methodSymbol.ContainingType.ToDisplayString() == CodeAssets.LoggerExtensionFullName
        };

        int index;
        for(index = 0; index < methodSymbol.Parameters.Length; index++)
        {
            var parameter = methodSymbol.Parameters[index];

            if(parameter.IsParams)
            {
                break;
            }

            if(parameter.Name == CodeAssets.ExceptionParameterName)
            {
                method.HasException = true;
            }
            else if(parameter.Name == CodeAssets.LogLevelParameterName)
            {
                method.HasLevel = true;
            }
        }

        foreach(var argument in invocation.ArgumentList.Arguments.Skip(index))
        {
            var symbol = TryParseExpression(argument.Expression, cts);

            if(TypeSymbolParser.TryParse(symbol, out var type))
            {
                var typeName = type!.ToDisplayString();

                if(type.IsTypeParameters)
                {
                    method.TypeArguments.Add(typeName, string.Join(", ", type.Constraints));
                }

                method.AddParameter(type);
            }
        }

        return method;
    }

    private static ISymbol? TryParseExpression(ExpressionSyntax syntax, GeneratorSyntaxContext cts)
    {
        return syntax switch
        {
            // 对象访问
            MemberAccessExpressionSyntax memberAccess
            => TryParseMemberAccessExpression(memberAccess, cts),

            // 变量名：user ( var user = new User(); )
            IdentifierNameSyntax identifierName
            => TryParseIndentifierName(identifierName, cts),

            // 创建对象
            ObjectCreationExpressionSyntax objectCreationExpression
            => cts.SemanticModel.GetSymbolInfo(objectCreationExpression.Type!).Symbol,

            // 常量
            LiteralExpressionSyntax literal
            => cts.SemanticModel.GetTypeInfo(literal).Type,

            // 调用
            InvocationExpressionSyntax invocationExpression
            => cts.SemanticModel.GetSymbolInfo(invocationExpression.Expression!).Symbol,

            // 强制转换
            CastExpressionSyntax castExpression
            => cts.SemanticModel.GetTypeInfo(castExpression).Type,

            // lambda
            ParenthesizedLambdaExpressionSyntax parenthesizedLambdaExpression
            => TryParseParenthesizedLambda(parenthesizedLambdaExpression, cts),

            // await
            AwaitExpressionSyntax awaitExpression
            => TryParseExpression(awaitExpression.Expression, cts),

            // array
            ArrayCreationExpressionSyntax arrayCreation
            => cts.SemanticModel.GetTypeInfo(arrayCreation).Type,

            ImplicitArrayCreationExpressionSyntax implicitArrayCreation
            => null,

            _ => throw new ArgumentException($"Unknown expression type: {syntax.GetType().FullName}"),
        };
    }

    private static ISymbol? TryParseParenthesizedLambda(ParenthesizedLambdaExpressionSyntax syntax, GeneratorSyntaxContext cts)
    {
        if(syntax.ReturnType is not null)
        {
            return cts.SemanticModel.GetSymbolInfo(syntax.ReturnType!).Symbol;
        }

        return syntax.Body switch
        {
            ExpressionSyntax expression => TryParseExpression(expression, cts),
            _ => throw new ArgumentException($"Unknown lambda expression type: {syntax.Body.GetType().FullName}"),
        };
    }

    private static ISymbol? TryParseMemberAccessExpression(MemberAccessExpressionSyntax syntax, GeneratorSyntaxContext cts)
    {
        var symbol = cts.SemanticModel.GetSymbolInfo(syntax!).Symbol;

        var kind = syntax.Name.Kind();

        var vt = syntax.Name.Identifier.ValueText;

        var n = symbol?.Name;

        if(symbol is not null
            && symbol.Name.Equals(syntax.Name.Identifier.ValueText, StringComparison.OrdinalIgnoreCase))
        {
            return symbol;
        }

        var name = syntax.Name.Identifier.ValueText;

        symbol = cts.SemanticModel.Compilation.GetSymbolsWithName(name, SymbolFilter.Member)
            .FirstOrDefault();

        if(symbol is null)
        {
            symbol = cts.SemanticModel.GetSymbolInfo(syntax.Expression).Symbol;

            return symbol switch
            {
                INamedTypeSymbol namedType => TrySeachMember(name, namedType),
                _ => throw new ArgumentException($"Unknown symbol type: {symbol?.GetType()}, {symbol?.ContainingNamespace}.{symbol?.Name}"),
            };
        }

        return symbol;
    }

    private static ISymbol? TrySeachMember(string name, INamedTypeSymbol model)
    {
        var symbol = model.GetMembers(name).FirstOrDefault();

        if(symbol is null && model.IsType && model.BaseType is not null)
        {
            return TrySeachMember(name, model.BaseType);
        }

        return symbol;
    }

    private static ISymbol? TryParseIndentifierName(IdentifierNameSyntax identifierName, GeneratorSyntaxContext cts)
    {
        var ti = cts.SemanticModel.GetTypeInfo(identifierName);
        if(ti.Type is not null
            and ITypeSymbol type)
        {
            return type;
        }

        var symbol = cts.SemanticModel.Compilation
            .GetSymbolsWithName(identifierName.Identifier.ValueText, SymbolFilter.Member)
            .FirstOrDefault();

        return symbol;
    }
}
