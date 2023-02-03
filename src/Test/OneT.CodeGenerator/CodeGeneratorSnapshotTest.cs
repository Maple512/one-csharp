namespace OneT.CodeGenerator;

using Basic.Reference.Assemblies;
using Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OneI;

/// <summary>
/// 代码生成器 快试帮助类
/// </summary>
public class CodeGeneratorSnapshotTest
{
    /// <summary>
    /// Verifies the.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="generator">The generator.</param>
    /// <param name="assemblyKind">The assembly kind.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="filePath">The file path.</param>
    /// <returns>A Task.</returns>
    protected static Task Verify(
        string source,
        IIncrementalGenerator generator,
        ReferenceAssemblyKind assemblyKind = ReferenceAssemblyKind.NetStandard20,
        Action<CSharpCompilationOptions>? configuration = null,
        [CallerFilePath] string? filePath = null)
    {
        var directory = Directory.GetCurrentDirectory();

        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.dll")
            .Select(x => MetadataReference.CreateFromFile(x));

        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        configuration?.Invoke(options);

        // 编译环境
        var compilation = CSharpCompilation.Create(
             "Tests",
             new[] { syntaxTree },
             references,
             options
             ).AddReferences(ReferenceAssemblies.Get(assemblyKind));

        var driver = CSharpGeneratorDriver.Create(generator);

        var folder = Path.Combine(TestTools.GetCSProjectDirecoty(filePath), "Logs");

        IOTools.EnsureDirectory(folder);

        var verify = Verifier.Verify(driver.RunGenerators(compilation))
            .UseDirectory(folder)
            .AutoVerify()
            .UseUniqueDirectory();

        return verify;
    }
}
