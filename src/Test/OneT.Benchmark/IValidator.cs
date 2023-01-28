namespace OneT.Benchmark;

using BenchmarkDotNet.Attributes;

public interface IValidator
{
    void Initialize() { }

    void Validate();

    [StackTraceHidden]
    public static void AreEquals<T>(T left, T right, IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        if(comparer.Equals(left, right) == false)
        {
            throw new ArgumentException("两个值不相等");
        }
    }
}
