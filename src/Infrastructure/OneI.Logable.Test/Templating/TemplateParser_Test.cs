namespace OneI.Logable.Templating;

using Templatizations;
using Templatizations.Tokenizations;

public class TemplateParser_Test
{
    [Fact]
    public void parse_text()
    {
        var text = "{Date:yyyy-MM-dd HH:mm:ss,-12'10}".AsMemory();

        var tokens = TemplateParser.Parse(text).ToList();

        tokens.Count.ShouldBe(1);

        tokens.First().ShouldBeAssignableTo<NamedPropertyToken>();
    }
}
/*
Total: 2.3000 μs, Avg: 127.7778 ns
1：200.0000 ns (TemplateParserBenchmark#L21 - TemplateParserBenchmark#L15)
2：200.0000 ns (TemplateParser#L15 - TemplateParser#L64)
3：100.0000 ns (TemplateParser#L64 - TemplateParser#L105)
4：100.0000 ns (TemplateParser#L105 - TemplateParser#L143)
5：100.0000 ns (TemplateParser#L143 - TemplateParser#L193)
6：0.0000 ns (TemplateParser#L193 - TemplateParser#L259)
7：200.0000 ns (TemplateParser#L259 - TemplateParser#L267)
8：100.0000 ns (TemplateParser#L267 - TemplateParser#L201)
9：100.0000 ns (TemplateParser#L201 - TemplateParser#L212)
10：100.0000 ns (TemplateParser#L212 - TemplateParser#L223)
11：0.0000 ns (TemplateParser#L223 - TemplateParser#L236)
12：200.0000 ns (TemplateParser#L236 - TemplateParser#L242)
13：300.0000 ns (TemplateParser#L242 - TemplateParser#L252)
14：100.0000 ns (TemplateParser#L252 - TemplateParser#L67)
15：300.0000 ns (TemplateParser#L67 - TemplateParser#L83)
16：200.0000 ns (TemplateParser#L83 - TemplateParser#L94)
17：0.0000 ns (TemplateParser#L94 - TemplateParser#L23)
*/
