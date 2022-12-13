namespace OneT.CodeGenerator;

using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis.CSharp;

/// <summary>
/// The C sharp compilation extensions.
/// </summary>
public static class CSharpCompilationExtensions
{
    public static CSharpCompilation AddReferences(this CSharpCompilation compilation, ReferenceAssemblyKind kind)
    {
        return compilation.AddReferences(ReferenceAssemblies.Get(kind));
    }
}
