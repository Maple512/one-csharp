namespace OneI.Utilityable;

using DotNext;

public class ObjectEqualsBenchmark : IValidator
{
    private const int count = 1;

#if DEBUG
    public void Validate()
    {
        var result = UseInt();

        Shouldly.ShouldBeTestExtensions.ShouldBe(UseEqualityComparerDefault(), result);
        Shouldly.ShouldBeTestExtensions.ShouldBe(UseEqualityComparerDefault_Short(), result);
        Shouldly.ShouldBeTestExtensions.ShouldBe(UseString(), result);
        Shouldly.ShouldBeTestExtensions.ShouldBe(UseReadOnlySpan(), result);
        Shouldly.ShouldBeTestExtensions.ShouldBe(UseDotNext(), result);
        Shouldly.ShouldBeTestExtensions.ShouldBe(UseMemory(), result);
    }
#endif

    [Benchmark(Baseline = true)]
    public int UseInt()
    {
        var array = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var result = 0;
        for(var i = 0; i < count; i++)
        {
            var index = 0;
            while(index < array.Length)
            {
                if(array[index] == '1')
                {
                    result = index;
                    break;
                }

                index++;
            }
        }

        return result;
    }

    [Benchmark]
    public int UseEqualityComparerDefault()
    {
        var array = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var result = 0;
        for(var i = 0; i < count; i++)
        {
            var index = 0;
            while(true)
            {
                if(EqualityComparer<char>.Default.Equals(array[index], '1'))
                {
                    result = index;
                    break;
                }

                index++;
            }
        }

        return result;
    }

    [Benchmark]
    public int UseEqualityComparerDefault_Short()
    {
        var array = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var result = 0;
        for(var i = 0; i < count; i++)
        {
            var index = 0;
            while(true)
            {
                var item = (short)array[index];
                if(EqualityComparer<short>.Default.Equals(item, (short)'1'))
                {
                    result = index;
                    break;
                }

                index++;
            }
        }

        return result;
    }

    [Benchmark]
    public int UseString()
    {
        var array = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var result = 0;
        for(var i = 0; i < count; i++)
        {
            result = array.IndexOf('1');
        }

        return result;
    }

    [Benchmark]
    public int UseReadOnlySpan()
    {
        var array = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".AsSpan();
        var result = 0;
        for(var i = 0; i < count; i++)
        {
            result = array.IndexOf('1');
        }

        return result;
    }

    [Benchmark]
    public int UseDotNext()
    {
        var array = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        var result = 0;
        for(var i = 0; i < count; i++)
        {
            for(var j = 0; j < array.Length; j++)
            {
                if(BitwiseComparer<char>.Equals(array[j], '1'))
                {
                    result = j;
                    break;
                }
            }
        }

        return result;
    }

    [Benchmark]
    public int UseMemory()
    {
        var array = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        var buffer = array.AsMemory();

        var result = 0;

        for(var i = 0; i < count; i++)
        {
            result = buffer.Span.IndexOf('1');
        }

        return result;
    }
}
