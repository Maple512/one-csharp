namespace OneI.Logable;

using OneI.Logable.Definitions;
/// <summary>
/// The invocation expression parser.
/// </summary>

public static class InvocationExpressionParser
{
    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="invocation">The invocation.</param>
    /// <param name="method">The method.</param>
    /// <param name="cts">The cts.</param>
    /// <param name="result">The result.</param>
    /// <returns>A bool.</returns>
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

    /// <summary>
    /// Parses the.
    /// </summary>
    /// <param name="invocation">The invocation.</param>
    /// <param name="methodSymbol">The method symbol.</param>
    /// <param name="cts">The cts.</param>
    /// <returns>A MethodDef.</returns>
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

    /// <summary>
    /// Tries the parse expression.
    /// </summary>
    /// <param name="syntax">The syntax.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>An ISymbol? .</returns>
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
            // 创建对象
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

        if(symbol is null or { Kind: SymbolKind.DynamicType })
        {
            return compilation.ObjectType;
        }

        return symbol;
    }

    /// <summary>
    /// Tries the parse default.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <returns>An ISymbol? .</returns>
    private static ISymbol? TryParseDefault(ExpressionSyntax expression, SemanticModel semanticModel)
    {
        var type = semanticModel.GetTypeInfo(expression).Type;
        var symbol = type ?? semanticModel.GetSymbolInfo(expression).Symbol;

        Debug.Assert(symbol != null);

        return type ?? symbol;
    }

    /// <summary>
    /// Tries the parse element access expression syntax.
    /// </summary>
    /// <param name="elementAccess">The element access.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <returns>An ISymbol? .</returns>
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
