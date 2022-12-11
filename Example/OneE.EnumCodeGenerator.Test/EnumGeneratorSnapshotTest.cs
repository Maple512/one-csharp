namespace OneE.EnumCodeGenerator.Test;

using System.Threading.Tasks;
using OneT.CodeGenerator;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class EnumGeneratorSnapshotTest : CodeGeneratorSnapshotTest
{
    [Fact]
    public Task GeneratesEnumExtensionsCorrectly()
    {
        // The source code to test
        var source = @"
using OneE.EnumCodeGenerator;

[EnumExtensions]
public enum Color
{
    Red = 0,
    Blue = 1,
}";

        // Pass the source code to our helper and snapshot test the output
        return Verify(source, new EnumToStringGenerator());
    }
}
