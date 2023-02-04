namespace OneI.Logable.Definitions;

using System.Collections.Generic;

internal class MethodDefComparer : IEqualityComparer<MethodDef>
{
    public static readonly IEqualityComparer<MethodDef> Instance = new MethodDefComparer();

    public bool Equals(MethodDef x, MethodDef y) => x.Equals(y);

    public int GetHashCode(MethodDef method) => method.GetHashCode();
}
