namespace OneI.Logable.Definitions;
/// <summary>
/// The parameter def.
/// </summary>

public class ParameterDef
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterDef"/> class.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="type">The type.</param>
    public ParameterDef(int index, TypeDef type)
        : this(index, $"p{index}", type) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterDef"/> class.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    public ParameterDef(int index, string name, TypeDef type)
    {
        Name = name;
        Index = index;
        Type = type;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the type.
    /// </summary>
    public TypeDef Type { get; }

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        return $"{Type} {Name}";
    }
}
