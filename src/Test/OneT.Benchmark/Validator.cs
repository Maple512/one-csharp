namespace OneT.Benchmark;

[StackTraceHidden]
public static class Validator
{
    public static void Equals<T>(this T left, T right, IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        if(comparer.Equals(left, right))
        {
            throw new ArgumentException("两个值不相等");
        }
    }
}
