namespace OneI.Generateable.Generator;

internal class TypeCache
{
    private static readonly ConcurrentDictionary<ISymbol, TypeDef> _types = new(EqualityComparer<ISymbol>.Default);

    public static void AddType(ISymbol symbol, TypeDef type)
    {
        if(_types.ContainsKey(symbol))
        {
            return;
        }

        var existed = _types.Values.FirstOrDefault(x => x.Equals(type));
        if(existed != null)
        {
            return;
        }

        _types[symbol] = type;
    }

    public static bool TryGetType(ISymbol symbol, [NotNullWhen(true)] out TypeDef? type)
    {
        return _types.TryGetValue(symbol, out type);
    }

    public static IEnumerable<TypeDef> Types => _types.Values;
}
