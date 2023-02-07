namespace OneI.Utilityable;

using System;
using System.Numerics;
using DotNext.Runtime;

public class IsDefualtBenchmark
{
    [Params(1000)]
    public int length;

    [Benchmark(Baseline = true)]
    public void UseDefault()
    {
        Sturct1 a = default;
        for(var i = 0; i < length; i++)
        {
            _ = a == default;
        }
    }

    [Benchmark]
    public void UseDotNext()
    {
        var a = new Sturct1();
        for(var i = 0; i < length; i++)
        {
            _ = Intrinsics.IsDefault(a);
        }
    }

    private struct Sturct1 : IEquatable<Sturct1>, IEqualityOperators<Sturct1, Sturct1, bool>
    {
        public bool Equals(Sturct1 other)
        {
            return Intrinsics.AreSame(this, other);
        }

        public override bool Equals(object? obj)
        {
            return obj is Sturct1 && Equals((Sturct1)obj);
        }

        public static bool operator ==(Sturct1 a, Sturct1 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Sturct1 a, Sturct1 b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
