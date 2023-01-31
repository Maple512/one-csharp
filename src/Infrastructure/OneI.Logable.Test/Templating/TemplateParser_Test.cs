namespace OneI.Logable.Templating;

using Shouldly;
using Templates;

public class TemplateParser_Test
{
    [Fact]
    public void parse_property()
    {
        var enumerator = new TemplateEnumerator("{Date:yyyy-MM-dd HH:mm:ss,-12'10}".AsMemory());

        while(enumerator.MoveNext())
        {
            Debug.WriteLine(enumerator.Current.ToString());
        }
    }

    [Theory]
    [InlineData("asdasdfasdf")]
    [InlineData("asdasdfasdf}")]
    [InlineData("asdasdfasdf{")]
    [InlineData("asdasdfasdf}asdfasdf")]
    [InlineData("asdasdfasdf{asdfasdf")]
    [InlineData("asdasdfasdf{{{{{{asdfasdf")]
    [InlineData("asdasdfasdf}}asdfasdf")]
    [InlineData("asdasdfasdf{}}}asdfasdf")]
    public void parse_text(string text)
    {
        var enumerator = new TemplateEnumerator(text.AsMemory());

        while(enumerator.MoveNext())
        {
            Debug.WriteLine(enumerator.Current.ToString());
        }
    }

    [Theory]
    [InlineData("as{{{f{0}", new[] { true, false })]
    [InlineData("as{{{f{0}}{a}{b} {{{c}}}", new[] { true, false, true, false, false, true, false, true })]
    [InlineData("{1}{2}{3}", new[] { false, false, false })]
    [InlineData("{1}{2-+-asdf:::,,,}{3}", new[] { false, true, false })]
    public void parse_admix(string text, bool[] expected)
    {
        var enumerator = new TemplateEnumerator(text.AsMemory());

        var index = 0;
        while(enumerator.MoveNext())
        {
            Debug.WriteLine(enumerator.Current);
            enumerator.Current.IsText().ShouldBe(expected[index], $"Index: {index}");
            ++index;
        }
    }
}
