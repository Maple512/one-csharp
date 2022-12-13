namespace System.Collections.Generic;

public class EnumerableExtensions_Test
{
    public static IEnumerable<object[]> GetEqualityComparers()
    {
        yield return new object[] { new[] { "a", "b", "c" }, EqualityComparer<string>.Default, 3 };

        yield return new object[] { new[] { "a", "a", "a" }, EqualityComparer<string>.Default, 1 };

        yield return new object[] { new[] { "a", "a", "A" }, StringComparer.OrdinalIgnoreCase, 1 };
    }

    [Theory]
    [MemberData(nameof(GetEqualityComparers))]
    public void exclude_null_and_writespace(IEnumerable<string> source, IEqualityComparer<string> comparer, int count)
        => source.ExcludeNullAndWriteSpace(comparer).Count().ShouldBe(count);
}
