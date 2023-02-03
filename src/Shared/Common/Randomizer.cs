namespace OneI;

#if NET
using System.Security.Cryptography;
using System.Text;
#endif

/// <summary>
/// 随机数
/// </summary>
[DebuggerStepThrough]
internal static partial class Randomizer
{
    private const string character_set = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_-+=[{]};:>|./?";

    public static string String()
    {
        return String(Integer(character_set.Length));
    }

    public static T Item<T>(params T[] objs)
    {
        _ = Check.NotNull(objs);

        return objs[Integer(objs.Length)];
    }

    public static T Item<T>(IEnumerable<T> list)
    {
        _ = Check.NotNull(list);

        return list.ElementAt(Integer(list.Count()));
    }

    public static IEnumerable<T> Items<T>(int count, params T[] source)
    {
        _ = Check.NotNull(source);

        for(var i = 0; i < count; i++)
        {
            yield return source[Integer(source.Length)];
        }

        yield break;
    }

    public static IEnumerable<T> Items<T>(IEnumerable<T> list, int count)
    {
        _ = Check.NotNull(list);

        for(var i = 0; i < count; i++)
        {
            yield return list.ElementAt(Integer(list.Count()));
        }

        yield break;
    }

    public static List<T> Disorder<T>(IEnumerable<T> items)
    {
        _ = Check.NotNull(items);

        var currentList = new List<T>(items);
        var randomList = new List<T>();

        while(currentList.Any())
        {
            var randomIndex = Integer(0, currentList.Count);

            randomList.Add(currentList[randomIndex]);

            currentList.RemoveAt(randomIndex);
        }

        return randomList;
    }

    public static string StringChinese(int length)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var gb2312 = Encoding.GetEncoding("gb2312");

        var bytes = "0123456789abcdef"u8.ToArray();

        var builder = new StringBuilder(length);

        for(var i = 0; i < length; i++)
        {
            var r1 = Integer(11, 14);
            var b1 = bytes[r1];

            //区位码第2位
            int r2;
            if(r1 == 13)
            {
                r2 = Integer(0, 7);
            }
            else
            {
                r2 = Integer(0, 16);
            }

            var b2 = bytes[r2];

            //区位码第3位
            var r3 = Integer(10, 16);
            var b3 = bytes[r3];
            var r4 = r3 switch
            {
                10 => Integer(1, 16),
                15 => Integer(0, 15),
                _ => Integer(0, 16),
            };
            var b4 = bytes[r4];

            var byte1 = Convert.ToByte(Encoding.UTF8.GetString(new[] { b1, b2 }), 16);
            var byte2 = Convert.ToByte(Encoding.UTF8.GetString(new[] { b3, b4 }), 16);

            var result = gb2312.GetString(new[] { byte1, byte2 });
            _ = builder.Append(result);
        }

        return builder.ToString();
    }
}

#if NET
[StackTraceHidden]
internal static partial class Randomizer
{
    public static string String(int length)
    {
        scoped Span<char> span = stackalloc char[length];

        for(var i = 0; i < length; i++)
        {
            span[i] = Item<char>(character_set);
        }

        return span.ToString();
    }

    public static int Integer(int minValue, int maxValue)
    {
        return RandomNumberGenerator.GetInt32(minValue, maxValue);
    }

    public static int Integer(int maxValue)
    {
        return RandomNumberGenerator.GetInt32(maxValue);
    }
}

#elif NETSTANDARD2_0_OR_GREATER
internal static partial class Randomizer
{
    private static readonly Random _random = new();

    public static string String(int length)
    {
        var span = new StringBuilder(length);

        for(var i = 0; i < length; i++)
        {
            _ = span.Append(Item<char>(character_set));
        }

        return span.ToString();
    }

    public static int Integer(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }

    public static int Integer(int maxValue)
    {
        return _random.Next(maxValue);
    }
}
#endif
