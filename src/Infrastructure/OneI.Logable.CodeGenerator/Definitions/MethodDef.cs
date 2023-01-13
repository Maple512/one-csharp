namespace OneI.Logable.Definitions;
/// <summary>
/// The method def.
/// </summary>

public class MethodDef
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodDef"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public MethodDef(string name) => Name = name;

    /// <summary>
    /// 表示该方法的类型参数和对应的约束
    /// </summary>
    public Dictionary<string, string?> TypeArguments { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether has level.
    /// </summary>
    public bool HasLevel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether has exception.
    /// </summary>
    public bool HasException { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is logger.
    /// </summary>
    public bool IsLogger { get; set; }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    public List<ParameterDef> Parameters { get; } = new();

    /// <summary>
    /// Adds the parameter.
    /// </summary>
    /// <param name="type">The type.</param>
    public void AddParameter(TypeDef type)
    {
        Parameters.Add(new ParameterDef(Parameters.Count, type));
    }
}
