namespace OneI.Generateable.Definitions;

using System;
using System.Collections.Generic;
using System.Linq;

internal class MethodDef : IEquatable<MethodDef>
{
    public MethodDef(string name) => Name = name;
    public string Name { get; }

    /// <summary>
    ///     表示该方法的类型参数和对应的约束
    /// </summary>
    public Dictionary<string, string?> TypeArguments { get; } = new();

    public List<ParameterDef> Parameters { get; } = new();

    public bool Equals(MethodDef other)
    {
        if(other == null)
        {
            return false;
        }

        return Name.Equals(other.Name, StringComparison.InvariantCulture)
               && TypeArguments.SequenceEqual(other.TypeArguments)
               && Parameters.SequenceEqual(other.Parameters);
    }

    public void AddParameter(TypeDef type) => Parameters.Add(new ParameterDef(Parameters.Count, type));

    public override bool Equals(object obj) => obj is MethodDef md && Equals(md);

    public bool Equals(MethodDef x, MethodDef y) => x.Equals(y);

    public int GetHashCode(MethodDef obj) => obj.GetHashCode();

    public override int GetHashCode()
    {
        var hashCode = 1825788138;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);

        foreach(var item in TypeArguments)
        {
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(item.Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<string?>.Default.GetHashCode(item.Value);
        }

        foreach(var item in Parameters)
        {
            hashCode = hashCode * -1521134295 + item.GetHashCode();
        }

        return hashCode;
    }
}
