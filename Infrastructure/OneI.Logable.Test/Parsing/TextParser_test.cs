namespace OneI.Logable.Parsing;

using System.Collections.Generic;
using System.Linq;

public class TextParser_test
{
    [Fact]
    public void parser_a_text()
    {
        var text1 = "{0}";
        var result1 = TextParser.Parse(text1);
        result1.GetCount().ShouldBe(1);
        result1.ElementAt(0).Position.ShouldBe(0);

        var text2 = "{a1}";
        var result2 = TextParser.Parse(text2);
        result2.GetCount().ShouldBe(1);
        result2.ElementAt(0).Position.ShouldBe(0);

        var text3 = " 0 {a} 1 {A} 2 {A} 3 {A} 4 {A} 5 ";
        // " 0 @ 1 @ 2 @ 3 @ 4 @ 5 "
        var result3 = TextParser.Parse(text3);

        result3.GetCount().ShouldBe(11);

        result3.ElementAt(0).Position.ShouldBe(0);
        result3.ElementAt(1).Position.ShouldBe(3);
        result3.ElementAt(2).Position.ShouldBe(4);
        result3.ElementAt(3).Position.ShouldBe(7);
        result3.ElementAt(4).Position.ShouldBe(8);
        result3.ElementAt(5).Position.ShouldBe(11);
        result3.ElementAt(6).Position.ShouldBe(12);
        result3.ElementAt(7).Position.ShouldBe(15);
        result3.ElementAt(8).Position.ShouldBe(16);
        result3.ElementAt(9).Position.ShouldBe(19);
        result3.ElementAt(10).Position.ShouldBe(20);

        var text4 = "1253411253411";
        var result4 = TextParser.Parse(text4);
        result4.GetCount().ShouldBe(1);
        result4.ElementAt(0).Position.ShouldBe(0);
    }

    [Fact]
    public void example1()
    {
        var text1 = "{{}{}{}{}{}}}}{{{{}}}}{2}}{}";

        var result1 = TextParser.Parse(text1);

        result1.GetCount().ShouldBe(3);

        result1.ElementAt(0).Position.ShouldBe(0);
        result1.ElementAt(1).Position.ShouldBe(22);
        result1.ElementAt(2).Position.ShouldBe(23);
    }

    [Fact]
    public void invalid_property()
    {
        var text1 = " 0 {@User}{$Name}{Age}{Date:yyyy-MM-dd}34{Date:yyyy-M:M-dd}";
        var result1 = TextParser.Parse(text1);
        result1.GetCount().ShouldBe(6);
        result1.ElementAt(0).ShouldBeAssignableTo<Token>();
        var property1 = (PropertyToken)result1.ElementAt(1);
        property1.Name.ShouldBe("User");
        property1.ParsingType.ShouldBe(PropertyTokenType.Deconstruct);
        property1.Format.ShouldBeNull();
        property1.ParameterIndex.ShouldBe(0);
        property1.Index.ShouldBe(0);

        result1.ElementAt(5).ShouldBeAssignableTo<Token>();
    }
}
