namespace OneI.Logable.Definitions;

public class MethodDef
{
    public string Name { get; }

    public MethodDef(string name) => Name = name;

    /// <summary>
    /// 表示该方法的类型参数和对应的约束
    /// </summary>
    public Dictionary<string, string?> TypeArguments { get; } = new();

    public bool HasLevel { get; set; }

    public bool HasException { get; set; }

    public List<ParameterDef> Parameters { get; } = new();

    public void AddParameter(TypeDef type)
    {
        Parameters.Add(new ParameterDef(Parameters.Count, type));
    }
}
