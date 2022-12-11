namespace OneI.Logable.Parsing;

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OneI.Logable.Definitions;
using OneI.Logable.Infrastructure;

public static class InvocationParser
{
    private static readonly ConcurrentDictionary<string, TypeDefinition> _types = new();

    public static MethodDefinition? TryParse(InvocationExpressionSyntax invocation, IMethodSymbol method, GeneratorSyntaxContext cts)
    {
        try
        {
            return Parse(invocation, method, cts);
        }
        catch(Exception ex)
        {
            throw;
        }
    }

    private static MethodDefinition Parse(InvocationExpressionSyntax invocation, IMethodSymbol methodSymbol, GeneratorSyntaxContext cts)
    {
        var method = new MethodDefinition(methodSymbol.Name);

        int index;
        for(index = 0; index < methodSymbol.Parameters.Length; index++)
        {
            var parameter = methodSymbol.Parameters[index];

            if(parameter.IsParams)
            {
                break;
            }

            var type = TypeParser.Parse(parameter.Type);

            if(parameter.Name == CodeAssets.ExceptionParameterName)
            {
                method.DefinedException();
            }
            else if(parameter.Name == CodeAssets.LogLevelParameterName)
            {
                method.DefinedLevel();
            }
        }

        foreach(var argument in invocation.ArgumentList.Arguments.Skip(index))
        {
            var symbol = TryParseExpression(argument.Expression, cts);

            var type = ParseSymbol(symbol, out var isTypeParameter);

            if(isTypeParameter)
            {
                method.AddTypeArgument(type.FullName, string.Join(", ", type.Constraints));
            }

            method.AddParameter(type!.FullName);
        }

        return method;
    }

    private static TypeDefinition ParseSymbol(ISymbol? symbol, out bool isTypeParameter)
    {
        isTypeParameter = false;

        if(symbol == null)
        {
            throw new Exception();
        }

        return symbol switch
        {
            IFieldSymbol field => TypeParser.Parse(field.Type),
            ILocalSymbol local => TypeParser.Parse(local.Type),
            IMethodSymbol method => TypeParser.Parse(method.ReturnType),
            ITypeSymbol type => TypeParser.Parse(type),
            IPropertySymbol property => TypeParser.Parse(property.Type),
            _ => throw new ArgumentException($"Unknown symbol type: {symbol.GetType().FullName}, {string.Join(", ", symbol.GetType().GetInterfaces().ToList())}"),
        };
    }

    private static TypeDefinition ParseTypeSymbol(ITypeSymbol symbol, bool isArrayType = false)
    {
        var fullName = $"global::{symbol.ContainingNamespace}.{symbol.Name}{(isArrayType ? "[]" : null)}";

        var symbolType = symbol.GetType().FullName;
        var st = symbol.SpecialType;
        var k = symbol.Kind;
        var tk = symbol.TypeKind;
        var str = symbol.ToDisplayString();

        var s = symbol.OriginalDefinition;

        var type = _types.Values.FirstOrDefault(x => x?.FullName == fullName);
        if(type is not null)
        {
            return type!;
        }

        if(symbol is IDynamicTypeSymbol dynamicType)
        {
            return new TypeDefinition("global::System.Object");
        }

        if(symbol is not INamedTypeSymbol typeSymbol)
        {
            throw new ArgumentException($"Unknown expression type: {symbolType}, {string.Join(", ", symbol.GetType().GetInterfaces().ToList())}");
        }

        type = new TypeDefinition(fullName);

        if(typeSymbol.IsGenericType)
        {
            foreach(var item in typeSymbol.TypeArguments)
            {
                var ct = ParseTypeSymbol(item);

                type.AddTypeArgument(ct!.FullName);
            }
        }

        if(isArrayType == false)
        {
            TryParsePropeties(typeSymbol, type);
        }
        else
        {
            type.ResetTypeKind(TypeKindEnum.Array);
        }

        _types.TryAdd(type.FullName, type);

        return type;
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

    private static void TryParsePropeties(INamedTypeSymbol symbol, TypeDefinition type)
    {
        var symbolType = symbol.GetType().FullName;
        var typeKind = symbol.TypeKind;
        var skind = symbol.SpecialType;
        var kink = symbol.Kind;

        var interfaces = symbol.Interfaces.Select(x => x.Name);

        switch(type.Name)
        {
            case "global::System.Nullable":
                type.ResetTypeKind(TypeKindEnum.Nullable);
                return;
            case "global::System.ValueTuple":
                type.ResetTypeKind(TypeKindEnum.ValueTuple);
                return;
            case "global::System.Exception":
            case "global::System.String":
                return;
        }

        if(interfaces.Any(x => x == nameof(Exception)))
        {
            return;
        }

        if(interfaces.Any(x => x == nameof(IDictionary)))
        {
            type.ResetTypeKind(TypeKindEnum.Dictionary);
            return;
        }

        if(interfaces.Any(x => x == nameof(IEnumerable)))
        {
            type.ResetTypeKind(TypeKindEnum.Enumerable);
            return;
        }

        if(IsTerminalType(symbol))
        {
            return;
        }

        type.ResetTypeKind(TypeKindEnum.Object);

        var propertySymbols = symbol.GetMembers().OfType<IPropertySymbol>()
                    .Where(x => x.DeclaredAccessibility == Accessibility.Public
                    && x.Parameters.IsDefaultOrEmpty);

        foreach(var item in propertySymbols)
        {
            var propertyType = ParseTypeSymbol(item.Type);

            type.AddProperty(new PropertyDefinition(item.Name, propertyType.FullName));
        }
    }

    public static void Wrap(IndentedStringBuilder builder, ImmutableArray<MethodDefinition?> methods)
    {
        builder.AppendLine("// <auto-generated/>");
        builder.AppendLine("#nullable enable");
        builder.AppendLine("namespace OneI.Logable;");
        builder.AppendLine();
        builder.AppendLine("[global::System.Diagnostics.DebuggerStepThrough]");
        builder.AppendLine("public static partial class Log");
        builder.AppendLine("{");

        using(var _ = builder.Indent())
        {
            foreach(var item in methods.Where(x => x is not null))
            {
                item!.AppendTo(builder, _types);

                builder.AppendLine();
            }

            builder.AppendLine("#region Create Property Values");
            builder.AppendLine();

            foreach(var item in _types)
            {
                item.Value.AppendTo(builder);

                builder.AppendLine();
            }

            builder.AppendLine("#endregion Create Property Values");
        }

        builder.AppendLine("}");
        builder.AppendLine("#nullable restore");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetTypeDefinition(string typeFullName, out TypeDefinition? typeDefinition)
    {
        typeDefinition = _types.Values.FirstOrDefault(x => x?.FullName == typeFullName);

        return typeDefinition is not null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsTerminalType(INamedTypeSymbol type)
    {
        return type.SpecialType != SpecialType.None
            || type.IsSerializable == false;
    }
}
