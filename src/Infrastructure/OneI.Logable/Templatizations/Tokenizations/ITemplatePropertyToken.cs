namespace OneI.Logable.Templatizations.Tokenizations;

using OneI.Logable.Rendering;

public interface ITemplatePropertyToken : ITemplateToken
{
    PropertyTokenType Type { get; }

    string? Format { get; }

    TextAlignment? Alignment { get; }

    int? Indent { get; }
}
