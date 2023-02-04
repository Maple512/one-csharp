namespace OneI.Logable;

using OneI.Logable.Templates;

public class size_test
{
    [Fact]
    public void size_of()
    {
        TestTools.PrintLayoutToFile<object>().Size.ShouldBe(8);

        TestTools.PrintLayoutToFile<PropertyValue>().Size.ShouldBe(16);

        TestTools.PrintLayoutToFile<TemplateHolder>().Size.ShouldBe(40);

        TestTools.PrintLayoutToFile<TemplateEnumerator>().Size.ShouldBe(112);

        TestTools.PrintLayoutToFile<ReadOnlyMemory<char>>().Size.ShouldBe(16);
    }
}
