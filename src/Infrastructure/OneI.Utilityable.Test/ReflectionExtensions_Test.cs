namespace OneI;

public class ReflectionExtensions_Test
{
    [Fact]
    public void get_type_display_name()
    {
        OneIReflectionExtensions.GetTypeDisplayName<Dictionary<string, string>>()
            .ShouldBe("System.Collections.Generic.Dictionary<string, string>");

        OneIReflectionExtensions.GetTypeDisplayName<Dictionary<string, string>>(false)
            .ShouldBe("Dictionary<string, string>");
    }
}
