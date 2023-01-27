namespace OneE.EnumCodeGenerator.Test;

using OneT.CodeGenerator;

/// <summary>
/// The enum generator snapshot test.
/// </summary>

[UsesVerify]
public class EnumGeneratorSnapshotTest : CodeGeneratorSnapshotTest
{
    /// <summary>
    /// Generates the enum extensions correctly.
    /// </summary>
    /// <returns>A Task.</returns>
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
