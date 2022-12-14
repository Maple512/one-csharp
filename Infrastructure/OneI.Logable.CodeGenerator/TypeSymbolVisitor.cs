namespace OneI.Logable;

using System.Collections;
using System.Linq;
using Microsoft.CodeAnalysis;
using OneI.Logable.Definitions;

/// <summary>
/// The type symbol visitor.
/// </summary>
public class TypeSymbolVisitor : SymbolVisitor
{
    /// <summary>
    /// The current.
    /// </summary>
    private TypeDef _current;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeSymbolVisitor"/> class.
    /// </summary>
    /// <param name="current">The current.</param>
    public TypeSymbolVisitor(TypeDef? current = null)
    {
        _current = current ?? new();
    }

    /// <summary>
    /// Try parse.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="type">The type.</param>
    /// <returns>A bool.</returns>
    public static bool TryParse(ISymbol? symbol, out TypeDef? type)
    {
        if(symbol == null)
        {
            type = null;
            return false;
        }

        var visitor = new TypeSymbolVisitor();

        visitor.Visit(symbol);

        type = visitor._current;

        if(type.IsTypeParameters == false)
        {
            CodePrinter.AddType(type);
        }

        return true;
    }

    /// <summary>
    /// Visit array type.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    public override void VisitArrayType(IArrayTypeSymbol symbol)
    {
        if(TryParse(symbol.ElementType, out var elementTyppe))
        {
            _current.Kind = TypeDefKind.Array;

            _current.Names.AddRange(elementTyppe!.Names);
        }
    }

    /// <summary>
    /// Visit dynamic type.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    public override void VisitDynamicType(IDynamicTypeSymbol symbol)
    {
        _current.Names.AddRange(new[] { nameof(System), nameof(Object) });
    }

    /// <summary>
    /// Visit type parameter.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    public override void VisitTypeParameter(ITypeParameterSymbol symbol)
    {
        _current.Names.Add(symbol.Name);

        _current.IsTypeParameters = true;

        if(symbol.HasConstructorConstraint)
        {
            _current.Constraints.Add("new()");
        }

        if(symbol.HasNotNullConstraint)
        {
            _current.Constraints.Add("notnull");
        }

        if(symbol.HasValueTypeConstraint)
        {
            _current.Constraints.Add("struct");
        }

        if(symbol.HasReferenceTypeConstraint)
        {
            _current.Constraints.Add("class");
        }

        if(symbol.HasUnmanagedTypeConstraint)
        {
            _current.Constraints.Add("unmanaged");
        }

        if(symbol.ConstraintTypes.IsDefaultOrEmpty == false)
        {
            foreach(var item in symbol.ConstraintTypes)
            {
                if(TryParse(item, out var constraintType))
                {
                    _current.Constraints.Add(constraintType!.ToDisplayString());
                }
            }
        }
    }

    /// <summary>
    /// Visit named type.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    public override void VisitNamedType(INamedTypeSymbol symbol)
    {
        try
        {
            Debug.WriteLine($"SpecialType: {symbol.SpecialType}, TypeKind: {symbol.TypeKind}, FullName: {symbol}, interfaces: {symbol.AllInterfaces.Select(x => $"{x}").Join()}", nameof(VisitNamedType));

            ParseNamedTypeNames(symbol);

            var name = _current.Names.Join('.');

            if(name == "System.String")
            {
                _current.Kind = TypeDefKind.Literal;
                return;
            }

            if(name == "System.Nullable")
            {
                _current.Kind = TypeDefKind.Nullable;
                return;
            }

            if(name == "System.ValueTuple")
            {
                _current.Kind = TypeDefKind.ValueTuple;

                foreach(var item in symbol.TupleElements)
                {
                    if(TryParse(item, out var tupleType))
                    {
                        _current.Properties.Add(new PropertyDef(item.Name, tupleType!));
                    }
                }

                return;
            }

            var fullName = _current.ToDisplayString();

            var interfaces = symbol.AllInterfaces.Select(x => x.Name).ToList();

            if(interfaces.Any(x => x == nameof(IDictionary)))
            {
                _current.Kind = TypeDefKind.Dictionary;
                return;
            }

            if(interfaces.Any(x => x == nameof(IEnumerable)))
            {
                _current.Kind = TypeDefKind.Enumerable;
                return;
            }

            if(symbol.IsSerializable)
            {
                var properties = symbol.GetMembers()
                    .Where(x => x.Kind == SymbolKind.Property && x.DeclaredAccessibility == Accessibility.Public);

                foreach(var item in properties)
                {
                    if(TryParse(item, out var propertyType))
                    {
                        _current.Properties.Add(new PropertyDef(item.Name, propertyType!));
                    }
                }

                _current.Kind = TypeDefKind.Object;
                return;
            }

            _current.Kind = TypeDefKind.Literal;
        }
        finally
        {
            if(_current.IsTypeParameters == false)
            {
                CodePrinter.AddType(_current);
            }
        }
    }

    /// <summary>
    /// Parses the named type names.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    private void ParseNamedTypeNames(INamedTypeSymbol symbol)
    {
        if(symbol.ContainingType is not null)
        {
            ParseNamedTypeNames(symbol.ContainingType);
        }
        else if(symbol.ContainingNamespace is not null)
        {
            VisitNamespace(symbol.ContainingNamespace);
        }

        if(symbol.IsGenericType)
        {
            foreach(var item in symbol.TypeArguments)
            {
                if(TryParse(item, out var argumentType))
                {
                    _current.TypeArguments.Add(argumentType!);
                }
            }
        }

        _current.Names.Add(symbol.Name);
    }

    /// <summary>
    /// Visit property.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    public override void VisitProperty(IPropertySymbol symbol)
    {
        TryParse(symbol.Type, out _current);
    }

    /// <summary>
    /// Visit namespace.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    public override void VisitNamespace(INamespaceSymbol symbol)
    {
        if(symbol.IsGlobalNamespace == false)
        {
            _current.Names.Insert(0, symbol.Name);

            VisitNamespace(symbol.ContainingNamespace);
        }
    }

    /// <summary>
    /// Visit field.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    public override void VisitField(IFieldSymbol symbol)
    {
        if(TryParse(symbol.Type, out var type))
        {
            _current = type!;
        }
    }

    /// <summary>
    /// Default the visit.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <exception cref="NotImplementedException"></exception>
    public override void DefaultVisit(ISymbol symbol)
    {
        throw new NotImplementedException(symbol.GetType().FullName);
    }
}