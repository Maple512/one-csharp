namespace OneI.Logable;

using System.Runtime.CompilerServices;
using System.Text.Json;

public class CommonTest
{
    [Fact]
    public void byte_type()
    {
        var result = string.Format("{0} {1}", 1, 2, 3, 4);

        var result1 = JsonSerializer.Serialize(TestHelpler.CreateNewUser());
    }

    [Fact]
    public void caller_attribute()
    {
        TriggerCaller();
    }

    private void TriggerCaller(
        [CallerMemberName] string? member = null,
        [CallerFilePath] string? file = null,
        [CallerLineNumber] int? line = null)
    {
        member.ShouldBe(nameof(caller_attribute));
        file!.EndsWith("CommonTest.cs").ShouldBeTrue();
        line.ShouldNotBeNull(); // 32
    }
}
