namespace OneI.Logable.Internal;

using OneI.Logable.Definitions;

internal static class TypeSymbolParser
{
    /// <summary>
    /// Object类型，适用于：dynamic, anonymous
    /// </summary>
    public static readonly TypeDef DefaultType = new(nameof(System), nameof(Object))
    {
        Kind = TypeDefKind.Literal,
    };

    public static bool TryParse(ISymbol? symbol, out TypeDef? type, int deep = 0)
    {
        type = DefaultType;

        if(symbol is null or IErrorTypeSymbol)
        {
            return true;
        }

        if(CodePrinter.TryGetType(symbol, out type))
        {
            return true;
        }

        type = symbol switch
        {
            IFieldSymbol field => Parse(field.Type, deep),
            ILocalSymbol local => Parse(local.Type, deep),
            IMethodSymbol method => Parse(method.ReturnType, deep),
            ITypeSymbol typeSymbol => Parse(typeSymbol, deep),
            IPropertySymbol property => Parse(property.Type, deep),
            _ => DefaultType,
        };

        return type is not null;
    }

    private static TypeDef Parse(ITypeSymbol symbol, int deep)
    {
        var type = symbol switch
        {
            IArrayTypeSymbol array => PraseArrayType(array, deep),
            IDynamicTypeSymbol _ => DefaultType,
            ITypeParameterSymbol typeParameter => PraseTypeParameter(typeParameter, deep),
            INamedTypeSymbol namedType => ParseNamedType(namedType, deep),
            _ => throw new ArgumentException($"Unknown type symbol: {symbol.GetType().FullName}, {string.Join(", ", symbol.GetType().GetInterfaces().ToList())}")
        };

        if(type.IsTypeParameters == false)
        {
            CodePrinter.AddType(symbol, type);
        }

        return type;
    }

    private static readonly string[] BuiltInTypes = new string[]
    {
        "System.Boolean",
        "System.Byte",
        "System.SByte",
        "System.Char",
        "System.Decimal",
        "System.Double",
        "System.Single",
        "System.Int32",
        "System.UInt32",
        "System.IntPtr",
        "System.UIntPtr",
        "System.Int64",
        "System.UInt64",
        "System.Int16",
        "System.UInt16",
        "System.Object",
        "System.String",
    };
    private static readonly string[] FormattableTypes = new string[] { "System.ISpanFormattable", "System.IFormattable", "System.ICustomFormatter" };
    private static readonly string[] ISpanFormattables = new string[]
    {
        "System.DateOnly",
        "System.TimeOnly",
        "System.DateTimeOffset" ,
        "System.Guid",
        "System.Half",
        "System.Int128",
        "System.Int32",
        "System.IntPtr",
        "System.Numerics.BigInteger",
        "System.Text.Rune",
        "System.UInt128",
        "System.UIntPtr",
    };

    private static TypeDef ParseNamedType(INamedTypeSymbol symbol, int deep)
    {
        if(symbol.IsAnonymousType)
        {
            return DefaultType;
        }

        var type = new TypeDef();

        ParseNamedTypeNames(symbol, type);
        ParseBaseTypes(symbol, type);

        var name = type.Names.Join('.');
        var interfaces = symbol.AllInterfaces.Select(x => x.ToDisplayString()).ToList();

        type.Interfaces.AddRange(interfaces);

        Debug.Write(name);

        if(type.BaseTypes.Contains("System.Exception") || name == "System.Exception")
        {
            type.IsException = true;
        }

        if(interfaces.Any(x => x == "OneI.Logable.Templates.IPropertyValueFormattable"))
        {
            type.Kind = TypeDefKind.Literal;
            return type;
        }
        if(interfaces.Any(x => x == "System.ISpanFormattable")
            || ISpanFormattables.Contains(type.ToString()))
        {
            type.Kind = TypeDefKind.Literal;
            type.IsSpanFormattable = true;
            return type;
        }
        if(interfaces.Any(x => x == "System.IFormattable"))
        {
            type.Kind = TypeDefKind.Literal;
            type.IsFormattable = true;
            return type;
        }
        if(BuiltInTypes.Contains(name))// 内置类型 
        {
            type.Kind = TypeDefKind.Literal;

            return type;
        }
        // 以上类型都是预处理的，不需要判断深度
        if(deep > 3)
        {
            type = DefaultType;

            return type;
        }

        if(name == "System.Nullable")
        {
            type.Kind = TypeDefKind.Nullable;

            return type;
        }

        if(name is "System.ValueTuple")
        {
            type.Kind = TypeDefKind.ValueTuple;

            ++deep;

            foreach(var item in symbol.TupleElements)
            {
                if(TryParse(item.Type, out var tupleType, deep))
                {
                    type.Properties.Add(new PropertyDef(item.Name, tupleType!));
                }
            }

            Debug.WriteLine(string.Empty);
            return type;
        }

        if(name is "System.Tuple")
        {
            type.Kind = TypeDefKind.Tuple;

            ++deep;

            foreach(var item in symbol.TypeArguments)
            {
                if(TryParse(item, out var tupleType, deep))
                {
                    type.Properties.Add(new PropertyDef(item.Name, tupleType!));
                }
            }

            Debug.WriteLine(string.Empty);
            return type;
        }

        Debug.Write(" | ");
        Debug.WriteLine(string.Join(", ", interfaces));

        if(interfaces.Any(x => x.StartsWith("System.Collections.Generic.IDictionary<")))
        {
            type.Kind = TypeDefKind.Dictionary;
            return type;
        }

        if(interfaces.Any(x => x.StartsWith("System.Collections.Generic.IEnumerable<")))
        {
            type.Kind = TypeDefKind.EnumerableT;
            return type;
        }

        if(interfaces.Any(x => x == "System.Collections.IEnumerable"))
        {
            type.Kind = TypeDefKind.Enumerable;
            return type;
        }

        if(symbol.IsSerializable)
        {
            var properties = symbol.GetMembers().OfType<IPropertySymbol>()
                                   .Where(x => x.Kind == SymbolKind.Property
                                                            && x.DeclaredAccessibility == Accessibility.Public
                                                            && x.GetMethod is not null
                                                            && x.IsIndexer == false);
            if(properties.Any())
            {
                deep++;
                foreach(var item in properties)
                {
                    if(TryParse(item.Type, out var propertyType, deep))
                    {
                        type.Properties.Add(new PropertyDef(item.Name, propertyType!));
                    }
                }
            }

            type.Kind = TypeDefKind.Object;

            return type;
        }

        type.Kind = TypeDefKind.Literal;

        return type;
    }

    private static TypeDef PraseArrayType(IArrayTypeSymbol symbol, int deep)
    {
        var elementType = Parse(symbol.ElementType, deep);

        var names = new List<string>(2 + elementType.Names.Count) { "System", "Array" };
        names.AddRange(elementType.Names);

        var result = new TypeDef(names)
        {
            Kind = TypeDefKind.Array,
        };

        result.TypeArguments.Add(elementType);

        return result;
    }

    private static TypeDef PraseTypeParameter(ITypeParameterSymbol symbol, int deep)
    {
        var type = new TypeDef(symbol.Name)
        {
            IsTypeParameters = true,
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
                var cdeep = 0;
                if(TryParse(item, out var constraintType, cdeep))
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

    private static void ParseBaseTypes(INamedTypeSymbol symbol, TypeDef type)
    {
        if(symbol.BaseType is null)
        {
            return;
        }

        type.BaseTypes.Add(symbol.BaseType.ToDisplayString());

        ParseBaseTypes(symbol.BaseType, type);
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
