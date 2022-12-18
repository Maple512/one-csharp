namespace OneI.Logable;

using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OneI.Logable.Definitions;

public static class InvocationExpressionParser
{
    public static bool TryParse(InvocationExpressionSyntax invocation, IMethodSymbol method, Compilation cts, out MethodDef? result)
    {
        result = null;

        try
        {
            result = Parse(invocation, method, cts);
        }
        catch(Exception)
        {
            throw;
        }

        return result != null;
    }

    private static MethodDef Parse(InvocationExpressionSyntax invocation, IMethodSymbol methodSymbol, Compilation cts)
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

    private static ISymbol? TryParseExpression(ExpressionSyntax syntax, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);

        var symbol = syntax switch
        {
            // default(int)
            DefaultExpressionSyntax defaultExpression
            => semanticModel.GetSymbolInfo(defaultExpression.Type).Symbol,
            // 对象访问
            MemberAccessExpressionSyntax memberAccess
            => semanticModel.GetSymbolInfo(memberAccess).Symbol,
            // 变量名：user ( var user = new User(); )
            IdentifierNameSyntax identifierName
            => semanticModel.GetSymbolInfo(identifierName).Symbol,
            // 创建对象
            ObjectCreationExpressionSyntax objectCreationExpression
            => semanticModel.GetSymbolInfo(objectCreationExpression.Type).Symbol,
            // lambda
            ParenthesizedLambdaExpressionSyntax parenthesizedLambdaExpression
            => semanticModel.GetSymbolInfo(parenthesizedLambdaExpression.ReturnType!).Symbol,

            // typeof
            TypeOfExpressionSyntax typeOfExpression
            => semanticModel.GetTypeInfo(typeOfExpression).Type,
            // 常量
            LiteralExpressionSyntax literal
            => semanticModel.GetTypeInfo(literal).Type,
            // 调用, nameof
            InvocationExpressionSyntax invocationExpression
            => semanticModel.GetTypeInfo(invocationExpression).Type,
            // 强制转换
            CastExpressionSyntax castExpression
            => semanticModel.GetTypeInfo(castExpression).Type,
            // struct with { Id = 1}
            WithExpressionSyntax withExpression
            => semanticModel.GetTypeInfo(withExpression).Type,
            // array IArrayTypeSymbol
            ArrayCreationExpressionSyntax arrayCreation
            => semanticModel.GetTypeInfo(arrayCreation.Type).Type,
            // new[] {1,2,3}
            ImplicitArrayCreationExpressionSyntax implicitArrayCreation
            => semanticModel.GetTypeInfo(implicitArrayCreation).Type,
            // new {Id = 1,Name = "Maple512", Age =18};
            AnonymousObjectCreationExpressionSyntax anonymousObject
            => semanticModel.GetTypeInfo(anonymousObject).Type,
            // await
            AwaitExpressionSyntax awaitExpression
            => semanticModel.GetTypeInfo(awaitExpression.Expression).Type,

            // array[0] or array[0..1]
            ElementAccessExpressionSyntax elementAccess
            => TryParseElementAccessExpressionSyntax(elementAccess, semanticModel),

            _ => TryParseDefault(syntax, semanticModel),
        };

        if(symbol is null or { Kind: SymbolKind.DynamicType })
        {
            return compilation.ObjectType;
        }

        return symbol;
    }

    //private static ISymbol? TryParseDefault(AwaitExpressionSyntax expression, SemanticModel semanticModel)
    //{
    //    var type = semanticModel.GetTypeInfo(expression).Type;
    //    var symbol = type ?? semanticModel.GetSymbolInfo(expression).Symbol;

    //    var type1 = semanticModel.GetTypeInfo(expression.Expression).Type;
    //    var symbol2 = type ?? semanticModel.GetSymbolInfo(expression.Expression).Symbol;

    //    Debug.Assert(symbol != null);

    //    return type ?? symbol;
    //}

    private static ISymbol? TryParseDefault(ExpressionSyntax expression, SemanticModel semanticModel)
    {
        var type = semanticModel.GetTypeInfo(expression).Type;
        var symbol = type ?? semanticModel.GetSymbolInfo(expression).Symbol;

        Debug.Assert(symbol != null);

        return type ?? symbol;
    }

    private static ISymbol? TryParseElementAccessExpressionSyntax(
        ElementAccessExpressionSyntax elementAccess,
        SemanticModel semanticModel)
    {
        var elementType = semanticModel.GetTypeInfo(elementAccess).Type;

        var arg = elementAccess.ArgumentList.Arguments[0].Expression;

        if(arg is RangeExpressionSyntax range)
        {
            return semanticModel.GetSymbolInfo(elementAccess.Expression).Symbol;
        }

        return elementType;
    }
}
