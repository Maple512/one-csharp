namespace OneI.Logable.Definitions;

using OneI.Logable.Infrastructure;

public class ParameterDefinition
{
    public ParameterDefinition(int index, string type, bool isException = false, bool isTypeParameter = false)
    {
        Name = $"p{index}";
        Index = index;
        Type = isTypeParameter ? type : $"global::{type}";
        IsTypeParameter = isTypeParameter;
        Kind = isException ? ParameterKind.Exception : ParseKind(type);
    }

    public string Name { get; }

    public int Index { get; }

    public string Type { get; }

    public ParameterKind Kind { get; }

    /// <summary>
    /// 泛型参数
    /// </summary>
    public bool IsTypeParameter { get; }

    public override string ToString() => $"{Type} {Name}";

    private static ParameterKind ParseKind(string type) => type switch
    {
        "OneI.Logable.LogLevel" => ParameterKind.LogLevel,
        "System.String" => ParameterKind.Message,
        _ => ParameterKind.None,
    };

    public void AppendTo(IndentedStringBuilder builder, TypeDefinition type)
    {

    }
}

public enum ParameterKind
{
    None, LogLevel, Message, Exception
}
