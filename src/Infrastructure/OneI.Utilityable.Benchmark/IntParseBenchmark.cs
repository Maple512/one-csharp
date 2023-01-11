namespace OneI.Utilityable;

using System.Globalization;
using DotNext;

public class IntParseBenchmark : IValidator
{
    private static readonly string number = "-87654321";

#if DEBUG
    public void Validate()
    {
        var result = UseSystem();

        Shouldly.ShouldBeTestExtensions.ShouldBe(UseDotNext(), result);
        Shouldly.ShouldBeTestExtensions.ShouldBe(UseCustome(), result);
    }
#endif

    [Benchmark(Baseline = true)]
    public int UseSystem()
    {
        return int.Parse(number.AsSpan(), NumberStyles.Integer, CultureInfo.InvariantCulture);
    }

    [Benchmark]
    public int UseDotNext()
    {
        return Number<int>.Parse(number);
    }

    [Benchmark]
    public int UseCustome()
    {
        return IntParseHelper.Parse(number);
    }

    private static class IntParseHelper
    {
        public static int Parse(scoped in ReadOnlySpan<char> text)
        {
            int nagate = 0;
            var result = 0;

            var index = text.Length;

            ref readonly var c = ref text[0];

            switch(c)
            {
                case '+':
                    index--;
                    break;
                case '-':
                    nagate = 1;
                    index--;
                    break;
            }

            do
            {
                c = ref text[^index];

                var num = c - '0';

                result += Pow(index - 1) * num;
            } while(index-- > 1);

            if(nagate == 1)
            {
                return -result;
            }

            return result;
        }

        public static int Pow(int length)
        {
            if(length == 0)
            {
                return 1;
            }

            var n = 1;
            var index = length;
            do
            {
                n *= 10;

            } while(index-- > 1);

            return n;
        }
    }
}
