namespace OneT.Benchmark;

using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

public abstract class BenchmarkItem
{
    /// <summary>
    /// 当前基准测试类中初始化方法，只会执行一次
    /// </summary>
    [GlobalSetup]
    public virtual void Inlitialize()
    {

    }

    [StackTraceHidden]
    protected static void SequenceEquals<T>(
        IEnumerable<T> left,
        IEnumerable<T> right,
        IEqualityComparer<T>? comparer = null,
        [CallerArgumentExpression(nameof(left))] string? leftEx = null,
        [CallerArgumentExpression(nameof(right))] string? rightEx = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        if(left.SequenceEqual(right, comparer) == false)
        {
            throw new ArgumentException($"\"{leftEx}\" != \"{rightEx}\"");
        }
    }

    [StackTraceHidden]
    protected static void AreEquals<T>(
        T left,
        T right,
        IEqualityComparer<T>? comparer = null,
        [CallerArgumentExpression(nameof(left))] string? leftEx = null,
        [CallerArgumentExpression(nameof(right))] string? rightEx = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        if(comparer.Equals(left, right) == false)
        {
            throw new ArgumentException($"\"{leftEx}\" != \"{rightEx}\"");
        }
    }
}
