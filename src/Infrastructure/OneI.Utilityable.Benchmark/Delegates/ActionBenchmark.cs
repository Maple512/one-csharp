namespace OneI.Utilityable.Delegates;

public class ActionBenchmark : BenchmarkItem
{
    [Params(1000)]
    public int length;

    [Benchmark(Baseline = true)]
    public void UseStaticLambda()
    {
        for(var i = 0; i < length; i++)
        {
#pragma warning disable IDE0039 // 使用本地函数
            Action<object> a = static args =>
            {
                var a = args;

                Console.WriteLine(a);
            };
#pragma warning restore IDE0039 // 使用本地函数

            a.Invoke(Randomizer.String(50));
        }
    }

    [Benchmark]
    public void UseLambda()
    {
        for(var i = 0; i < length; i++)
        {
#pragma warning disable IDE0039 // 使用本地函数
            Action<object> a = args =>
            {
                var a = args;

                Console.WriteLine(a);
            };
#pragma warning restore IDE0039 // 使用本地函数

            a.Invoke(Randomizer.String(50));
        }
    }

    [Benchmark]
    public void UseStaticMethod()
    {
        for(var i = 0; i < length; i++)
        {
            Action<object> a = StaticMethod1;

            a.Invoke(Randomizer.String(50));
        }
    }

    [Benchmark]
    public void UseMethod()
    {
        for(var i = 0; i < length; i++)
        {
            Action<object> a = StaticMethod2;

            a.Invoke(Randomizer.String(50));
        }
    }

    private static void StaticMethod1(object args)
    {
        var a = args;
        Console.WriteLine(a);
    }

    private void StaticMethod2(object args)
    {
        var a = args;
        Console.WriteLine(a);
    }
}
