namespace OneI.Logable.Parsing;

using System;
using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using OneI.Logable.Definitions;

public static class TypeParser
{
    private static readonly ConcurrentDictionary<string, TypeDefinition> _types = new();

    public static TypeDefinition Parse(ITypeSymbol symbol)
    {
        var name = $"{symbol.ContainingNamespace}.{symbol.Name}";

        if(_types.TryGetValue(name, out var type) == false)
        {
            type = symbol switch
            {
                IArrayTypeSymbol array => PraseArrayType(array),
                IDynamicTypeSymbol _ => PraseDynamicType(),
                ITypeParameterSymbol typeParameter => PraseTypeParameter(typeParameter),
                INamedTypeSymbol namedType => ParseNamedType(namedType),
                _ => throw new ArgumentException($"Unknown type symbol: {symbol.GetType().FullName}, {string.Join(", ", symbol.GetType().GetInterfaces().ToList())}"),
            };

            if(type.IsTypeArguments == false)
            {
                _types[name] = type;
            }
        }

        return type;
    }

    private static TypeDefinition ParseNamedType(INamedTypeSymbol symbol)
    {
        var fullName = $"{symbol.ContainingNamespace}.{symbol.Name}";

        Debug.WriteLine($"SpecialType: {symbol.SpecialType}, TypeKind: {symbol.TypeKind}, FullName: {fullName}, interfaces: {symbol.AllInterfaces.Select(x => $"{x.ContainingNamespace}.{x.Name}").Join()}", nameof(ParseNamedType));

        // 待实现：System_MulticastDelegate System_Delegate
        switch(fullName)
        {
            case "System.Nullable":
                break;
            case "System.ValueTuple":
                break;
            case "System.Exception":
            case "System.String":
                break;
        }

        //if(symbol.SpecialType == SpecialType.None
        //     || symbol.IsSerializable == false)
        //{
        //    return new TypeDefinition(name, false, TypeKindEnum.Literal);
        //}

        // var interfaces = symbol.Interfaces.Select(x => x.Name);

        return new TypeDefinition(fullName);
    }

    private static TypeDefinition PraseArrayType(IArrayTypeSymbol symbol)
    {
        var elementType = Parse(symbol.ElementType);

        return new TypeDefinition($"{elementType}[]", false, TypeKindEnum.Array);
    }

    private static TypeDefinition PraseDynamicType() => new("System.Object");

    private static TypeDefinition PraseTypeParameter(ITypeParameterSymbol symbol)
    {
        var type = new TypeDefinition(symbol.Name, true);

        if(symbol.HasConstructorConstraint)
        {
            type.AddConstraint("new()");
        }

        if(symbol.HasNotNullConstraint)
        {
            type.AddConstraint("notnull");
        }

        if(symbol.HasValueTypeConstraint)
        {
            type.AddConstraint("struct");
        }

        if(symbol.HasReferenceTypeConstraint)
        {
            type.AddConstraint("class");
        }

        if(symbol.HasUnmanagedTypeConstraint)
        {
            type.AddConstraint("unmanaged");
        }

        if(symbol.ConstraintTypes.IsDefaultOrEmpty == false)
        {
            foreach(var item in symbol.ConstraintTypes)
            {
                type.AddConstraint(Parse(item).ToDisplayString());
            }
        }

        return type;
    }
}
