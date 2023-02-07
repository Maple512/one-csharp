namespace OneI.Utilityable.Strings;

public class StringHashCodeBenchmark : BenchmarkItem
{
    private const int count = 100;

    private string s1
    {
        get
        {
            return $"0123456789abcdasdjf{s3}/+9as8d+f*a/sd+fasd+f9";
        }
    }

    private string s2
    {
        get
        {
            return $"0123456789abcdasdjf{s3}/+9as8d+f*a/sd+fasd+f9";
        }
    }

    private string s3
    {
        get
        {
            return "0123456789abcdasdjfaopi12+-*/+9as8d+f*a/sd+fasd+f9";
        }
    }

    [Benchmark(Baseline = true)]
    public bool UseStringEquals()
    {
        var result = false;

        for(var i = 0; i < 100; i++)
        {
            result = s1.Equals(s2, StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }

    [Benchmark]
    public bool UseStringCompare()
    {
        var result = false;

        for(var i = 0; i < 100; i++)
        {
            result = string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        return result;
    }

    [Benchmark]
    public bool UseStringHashCode()
    {
        var result = false;

        for(var i = 0; i < 100; i++)
        {
            var h1 = s1.GetHashCode();
            var h2 = s2.GetHashCode();

            result = h1 == h2;
        }

        return result;

    }

    [Benchmark]
    public bool UseEqualityComparerDefault()
    {
        var result = false;

        for(var i = 0; i < 100; i++)
        {
            result = EqualityComparer<string>.Default.Equals(s1, s2);
        }

        return result;
    }

    [Benchmark]
    public bool UseSpan_SequenceEqual()
    {
        var result = false;

        for(var i = 0; i < 100; i++)
        {
            result = s1.AsSpan().SequenceEqual(s2);
        }

        return result;
    }

    [Benchmark]
    public bool UseSpan_CompareTo()
    {
        var result = false;

        for(var i = 0; i < 100; i++)
        {
            result = s1.AsSpan().CompareTo(s2, StringComparison.InvariantCulture) == 0;
        }

        return result;
    }

    [Benchmark]
    public bool UseSpan_Equals()
    {
        var result = false;

        for(var i = 0; i < 100; i++)
        {
            result = s1.AsSpan().Equals(s2, StringComparison.InvariantCulture);
        }

        return result;
    }

    public override void Inlitialize()
    {
        var result = UseStringEquals();

        if(Unsafe.AreSame(ref MemoryMarshal.GetReference(s1.AsSpan()), ref MemoryMarshal.GetReference(s2.AsSpan())))
        {
            throw new Exception();
        }

        AreEquals(UseStringHashCode(), result);
        AreEquals(UseSpan_SequenceEqual(), result);
        AreEquals(UseSpan_Equals(), result);
        AreEquals(UseEqualityComparerDefault(), result);
    }
}
