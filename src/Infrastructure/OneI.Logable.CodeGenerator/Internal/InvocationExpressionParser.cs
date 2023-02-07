namespace OneI.Logable.Internal;

using System;
using Microsoft.CodeAnalysis;
using OneI.Logable.Definitions;

internal static class InvocationExpressionParser
{
    public static bool TryParse(TargetContext context, Compilation cts, out MethodDef? method)
    {
        method = null;
        try
        {
            method = new MethodDef(context.Method!.Name);

            int index;
            for(index = 0; index < context.Method.Parameters.Length; index++)
            {
                var parameter = context.Method.Parameters[index];

                if(parameter.IsParams)
                {
                    break;
                }

                switch(parameter.Name)
                {
                    case CodeAssets.ExceptionParameterName:
                        method.HasException = true;
                        break;
                    case CodeAssets.LogLevelParameterName:
                        method.HasLevel = true;
                        break;
                    case CodeAssets.MessageParameterName:
                        method.HasMessage = true;
                        break;
                }
            }

            foreach(var argument in context.SyntaxNode.ArgumentList.Arguments.Skip(index))
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

            return true;
        }
        catch(Exception)
        {
            return false;
        }
    }

    private static ISymbol? TryParseExpression(ExpressionSyntax syntax, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);

        var symbol = syntax switch
        {
            // default(int)
            DefaultExpressionSyntax expression
                => semanticModel.GetSymbolInfo(expression.Type).Symbol,
            // 对象访问
            MemberAccessExpressionSyntax expression
                => semanticModel.GetSymbolInfo(expression).Symbol,
            // 变量名：user ( var user = new User(); )
            IdentifierNameSyntax expression
                => semanticModel.GetSymbolInfo(expression).Symbol,
            ObjectCreationExpressionSyntax expression
                => semanticModel.GetSymbolInfo(expression.Type).Symbol,
            // lambda
            ParenthesizedLambdaExpressionSyntax expression
                => semanticModel.GetSymbolInfo(expression.ReturnType!).Symbol,

            // typeof
            TypeOfExpressionSyntax expression
                => semanticModel.GetTypeInfo(expression).Type,
            // 常量
            LiteralExpressionSyntax expression
                => semanticModel.GetTypeInfo(expression).Type,
            // 调用, nameof
            InvocationExpressionSyntax expression
                => semanticModel.GetTypeInfo(expression).Type,
            // 强制转换
            CastExpressionSyntax expression
                => semanticModel.GetTypeInfo(expression).Type,
            // struct with { Id = 1}
            WithExpressionSyntax expression
                => semanticModel.GetTypeInfo(expression).Type,
            // array IArrayTypeSymbol
            ArrayCreationExpressionSyntax expression
                => semanticModel.GetTypeInfo(expression).Type,
            // new[] {1,2,3}
            ImplicitArrayCreationExpressionSyntax expression => semanticModel.GetTypeInfo(expression).Type,
            // new {Id = 1,Name = "Maple512", Age =18};
            AnonymousObjectCreationExpressionSyntax expression => semanticModel.GetTypeInfo(expression).Type,
            // await
            AwaitExpressionSyntax expression => semanticModel.GetTypeInfo(expression).Type,

            // array[0] or array[0..1]
            ElementAccessExpressionSyntax expression
                => TryParseElementAccessExpressionSyntax(expression, semanticModel),

            _ => TryParseDefault(syntax, semanticModel),
        };

        if(symbol is null or { Kind: SymbolKind.DynamicType, })
        {
            return compilation.ObjectType;
        }

        return symbol;
    }

    private static ISymbol? TryParseDefault(ExpressionSyntax expression, SemanticModel semanticModel)
    {
        var type = semanticModel.GetTypeInfo(expression).Type;
        var symbol = type ?? semanticModel.GetSymbolInfo(expression).Symbol;

        Debug.Assert(symbol != null);

        return type ?? symbol;
    }

    private static ISymbol? TryParseElementAccessExpressionSyntax(
        ElementAccessExpressionSyntax elementAccess, SemanticModel semanticModel)
    {
        var elementType = semanticModel.GetTypeInfo(elementAccess).Type;

        var arg = elementAccess.ArgumentList.Arguments[0].Expression;

        if(arg is RangeExpressionSyntax)
        {
            return semanticModel.GetSymbolInfo(elementAccess.Expression).Symbol;
        }

        return elementType;
    }
}
