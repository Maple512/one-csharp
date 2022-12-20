namespace OneT.CodeGenerator;

using System.Threading.Tasks;
using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OneT.Common;
using VerifyXunit;

/// <summary>
/// 代码生成器 快试帮助类
/// </summary>
public class CodeGeneratorSnapshotTest
{
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
             ).AddReferences(assemblyKind);

        var driver = CSharpGeneratorDriver.Create(generator);

        var verify = Verifier.Verify(driver.RunGenerators(compilation))
            .UseDirectory(Path.Combine(TestTools.GetCSProjectDirecoty(filePath), "Logs"))
            .AutoVerify()
            .UseUniqueDirectory();

        return verify;
    }
}
