namespace OneT.CodeGenerator;

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;

/// <summary>
/// 代码生成器 快照测试帮助类
/// </summary>
public class GeneratorSnapshotTest
{
    protected static Task Verify(
        string source,
        IIncrementalGenerator generator,
        List<MetadataReference>? references = null,
        [CallerFilePath] string? callFilePath = null)
    {
        // Parse the provided string into a C# syntax tree
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        references ??= new List<MetadataReference>();

        references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

        // Create a Roslyn compilation for the syntax tree.
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references);

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver = driver.RunGenerators(compilation);

        // 去掉：bin/Debug/net7.0/，定位到项目目录下
        var projectLocation = Path.GetDirectoryName(callFilePath)!;

        // Use verify to snapshot test the source generator output!
        var verify = Verifier.Verify(driver)
            .UseDirectory(Path.Combine(projectLocation, "Generate Results"))
            .AutoVerify()
            .UseUniqueDirectory();

        return verify;
    }
}
