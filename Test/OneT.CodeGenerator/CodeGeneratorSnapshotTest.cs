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
        [CallerFilePath] string? filePath = null)
    {
        var directory = Directory.GetCurrentDirectory();

        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = Directory.EnumerateFiles(Directory.GetCurrentDirectory(),"*.dll")
            .Select(x=>MetadataReference.CreateFromFile(x));

        // 编译环境
        var compilation = CSharpCompilation.Create(
             "Tests",
             new[] { syntaxTree },
             references,
             new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
             ).AddReferences(assemblyKind);

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        var verify = Verifier.Verify(driver)
            .UseDirectory(Path.Combine(TestTools.GetCSProjectDirecoty(filePath), "Logs"))
            .AutoVerify()
            .UseUniqueDirectory();

        return verify;
    }
}
