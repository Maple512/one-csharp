namespace OneI.Logable;

using System;
using System.Collections;
using Microsoft.CodeAnalysis;
using OneI.Logable.Definitions;

public static class TypeSymbolParser
{
    public static bool TryParse(ISymbol? symbol, out TypeDef? type)
    {
        type = null;
        if(symbol == null)
        {
            return false;
        }

        type = symbol switch
        {
            IFieldSymbol field => Parse(field.Type),
            ILocalSymbol local => Parse(local.Type),
            IMethodSymbol method => Parse(method.ReturnType),
            ITypeSymbol typeSymbol => Parse(typeSymbol),
            IPropertySymbol property => Parse(property.Type),
            _ => throw new ArgumentException($"Unknown symbol type: {symbol.GetType().FullName}, {string.Join(", ", symbol.GetType().GetInterfaces().ToList())}"),
        };

        return type is not null;
    }

    public static TypeDef Parse(ITypeSymbol symbol)
    {
        var type = symbol switch
        {
            IArrayTypeSymbol array => PraseArrayType(array),
            IDynamicTypeSymbol _ => PraseDynamicType(),
            ITypeParameterSymbol typeParameter => PraseTypeParameter(typeParameter),
            INamedTypeSymbol namedType => ParseNamedType(namedType),
            _ => throw new ArgumentException($"Unknown type symbol: {symbol.GetType().FullName}, {string.Join(", ", symbol.GetType().GetInterfaces().ToList())}"),
        };

        if(type.IsTypeParameters == false)
        {
            CodePrinter.AddType(type);
        }

        return type;
    }

    private static TypeDef ParseNamedType(INamedTypeSymbol symbol)
    {
        var type = new TypeDef();
        try
        {

            Debug.WriteLine($"SpecialType: {symbol.SpecialType}, TypeKind: {symbol.TypeKind}, FullName: {symbol}, interfaces: {symbol.AllInterfaces.Select(x => $"{x}").Join()}", nameof(ParseNamedType));

            ParseNamedTypeNames(symbol, type);

            var name = type.Names.Join('.');

            if(name == "System.String")
            {
                type.Kind = TypeDefKind.Literal;
                return type;
            }

            if(name == "System.Nullable")
            {
                type.Kind = TypeDefKind.Nullable;
                return type;
            }

            if(name == "System.ValueTuple")
            {
                type.Kind = TypeDefKind.ValueTuple;

                foreach(var item in symbol.TupleElements)
                {
                    if(TryParse(item.Type, out var tupleType))
                    {
                        type.Properties.Add(new PropertyDef(item.Name, tupleType!));
                    }
                }

                return type;
            }

            var fullName = type.ToDisplayString();

            var interfaces = symbol.AllInterfaces.Select(x => x.Name).ToList();

            if(interfaces.Any(x => x == nameof(IDictionary)))
            {
                type.Kind = TypeDefKind.Dictionary;
                return type;
            }

            if(interfaces.Any(x => x == nameof(IEnumerable)))
            {
                type.Kind = TypeDefKind.Enumerable;
                return type;
            }

            if(symbol.IsSerializable)
            {
                var properties = symbol.GetMembers()
                    .Where(x => x.Kind == SymbolKind.Property && x.DeclaredAccessibility == Accessibility.Public)
                    .OfType<IPropertySymbol>();

                foreach(var item in properties)
                {
                    if(TryParse(item.Type, out var propertyType))
                    {
                        type.Properties.Add(new PropertyDef(item.Name, propertyType!));
                    }
                }

                type.Kind = TypeDefKind.Object;

                return type;
            }

            type.Kind = TypeDefKind.Literal;

        }
        finally
        {
            if(type.IsTypeParameters == false)
            {
                CodePrinter.AddType(type);
            }
        }

        return type;
    }

    private static TypeDef PraseArrayType(IArrayTypeSymbol symbol)
    {
        var elementType = Parse(symbol.ElementType);

        return new TypeDef(elementType.Names)
        {
            Kind = TypeDefKind.Array
        };
    }

    private static TypeDef PraseDynamicType()
    {
        return new(nameof(System), nameof(Object));
    }

    private static TypeDef PraseTypeParameter(ITypeParameterSymbol symbol)
    {
        var type = new TypeDef(symbol.Name)
        {
            IsTypeParameters = true
        };

        if(symbol.HasConstructorConstraint)
        {
            type.Constraints.Add("new()");
        }

        if(symbol.HasNotNullConstraint)
        {
            type.Constraints.Add("notnull");
        }

        if(symbol.HasValueTypeConstraint)
        {
            type.Constraints.Add("struct");
        }

        if(symbol.HasReferenceTypeConstraint)
        {
            type.Constraints.Add("class");
        }

        if(symbol.HasUnmanagedTypeConstraint)
        {
            type.Constraints.Add("unmanaged");
        }

        if(symbol.ConstraintTypes.IsDefaultOrEmpty == false)
        {
            foreach(var item in symbol.ConstraintTypes)
            {
                if(TryParse(item, out var constraintType))
                {
                    type.Constraints.Add(constraintType!.ToDisplayString());
                }
            }
        }

        return type;
    }

    private static void ParseNamedTypeNames(INamedTypeSymbol symbol, TypeDef type)
    {
        if(symbol.ContainingType is not null)
        {
            ParseNamedTypeNames(symbol.ContainingType, type);
        }
        else if(symbol.ContainingNamespace is not null)
        {
            ParseNamespace(symbol.ContainingNamespace, type);
        }

        if(symbol.IsGenericType)
        {
            foreach(var item in symbol.TypeArguments)
            {
                if(TryParse(item, out var argumentType))
                {
                    type.TypeArguments.Add(argumentType!);
                }
            }
        }

        type.Names.Add(symbol.Name);
    }

    public static void ParseNamespace(INamespaceSymbol symbol, TypeDef type)
    {
        if(symbol.IsGlobalNamespace == false)
        {
            type.Names.Insert(0, symbol.Name);

            ParseNamespace(symbol.ContainingNamespace, type);
        }
    }
}
