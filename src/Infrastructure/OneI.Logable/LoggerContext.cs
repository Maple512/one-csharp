namespace OneI.Logable;

using OneI.Logable.Templatizations;
using OneI.Logable.Templatizations.Tokenizations;

public class LoggerContext
{
    internal LoggerContext(
        IReadOnlyList<ITemplateToken> tokens,
        IReadOnlyList<ITemplateProperty> properties,
        LoggerMessageContext messageContext)
    {
        Tokens = tokens;
        Properties = properties;
        MessageContext = messageContext;
    }

    public LoggerMessageContext MessageContext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public IReadOnlyList<ITemplateToken> Tokens { get; }

    public IReadOnlyList<ITemplateProperty> Properties { get; }

    public bool TryGetValue(string name, out PropertyValue? value)
    {
        var propert = Properties.OfType<NamedProperty>()
            .FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCulture));

        if(propert != null && propert.Value is PropertyValue pv)
        {
            value = pv;

            return true;
        }

        value = default;

        return false;
    }

    public bool TryGetValue<T>(string name, out T? value)
    {
        var propert = Properties.OfType<NamedProperty>()
            .FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCulture));

        if(propert != null && propert.Value is LiteralValue<T> lv)
        {
            value = lv.Value;

            return true;
        }

        value = default;

        return false;
    }
}
