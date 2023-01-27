namespace OneI;

public class ValueDictionary_Test
{
    [Fact]
    public void add_items()
    {
        var dic = new ValueDictionary<int, int>();

        dic.Add(1, 2);

        dic.TryGetValue(1, out int value).ShouldBeTrue();

        value.ShouldBe(2);

        var d2 = new ValueDictionary<int, int>(dic);

        d2.Length.ShouldBe(1);
    }
}
