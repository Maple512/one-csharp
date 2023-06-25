namespace OneI;

#if NET
using System.Security.Cryptography;
using System.Text;
#endif

/// <summary>
/// 随机数
/// </summary>
[StackTraceHidden]
[DebuggerStepThrough]
internal static partial class Randomizer
{
    private const string latter = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string characters = $"0123456789{latter}!@#$%^&*()_-+=[{{]}};:>|./?";

    #region Enumerable

    /// <summary>
    /// 从给定集合中随机获取一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T Item<T>(params T[] source)
    {
        Check.ThrowIfNull(source);

        return source[Integer(source.Length)];
    }

    /// <summary>
    /// 从给定集合中随机获取一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T Item<T>(IEnumerable<T> source)
    {
        Check.ThrowIfNull(source);

        return source.ElementAt(Integer(source.Count()));
    }

    /// <summary>
    /// 从给定集合中随机获取指定长度的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="count"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T> Items<T>(int count, params T[] source)
    {
        Check.ThrowIfNull(source);

        for(var i = 0; i < count; i++)
        {
            yield return source[Integer(source.Length)];
        }

        yield break;
    }

    /// <summary>
    /// 从给定集合中随机获取指定长度的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static IEnumerable<T> Items<T>(IEnumerable<T> source, int count)
    {
        Check.ThrowIfNull(source);

        for(var i = 0; i < count; i++)
        {
            yield return source.ElementAt(Integer(source.Count()));
        }

        yield break;
    }

    /// <summary>
    /// 打乱顺序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    public static IEnumerable<T> Disorder<T>(IEnumerable<T> items)
    {
        Check.ThrowIfNull(items);

        var currentList = new List<T>(items);
        var randomList = new List<T>(currentList.Count);

        while(currentList.Any())
        {
            var randomIndex = Integer(currentList.Count);

            randomList.Add(currentList[randomIndex]);

            currentList.RemoveAt(randomIndex);
        }

        return randomList;
    }

    #endregion

    #region String

    /// <summary>
    /// 随机字母
    /// </summary>
    /// <returns></returns>
    public static string Latter()
    {
        return Latter(Integer(latter.Length));
    }

    /// <summary>
    /// 获取指定长度的随机字母
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string Latter(int length)
    {
        scoped Span<char> span = stackalloc char[length];

        for(var i = 0; i < length; i++)
        {
            span[i] = Item<char>(latter);
        }

        return span.ToString();
    }

    /// <summary>
    /// 随机字符
    /// </summary>
    /// <returns></returns>
    public static string String()
    {
        return String(Integer(characters.Length));
    }

    /// <summary>
    /// 获取指定长度的随机字符
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string String(int length)
    {
        scoped Span<char> span = stackalloc char[length];

        for(var i = 0; i < length; i++)
        {
            span[i] = Item<char>(characters);
        }

        return span.ToString();
    }

    /// <summary>
    /// 随机中文文字
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
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

    #endregion

    #region Numberic

    /// <summary>
    /// 随机偶数
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Even(int max)
    {
        return Integer(max) & ~1;
    }

    /// <summary>
    /// 随机偶数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Even(int min, int max)
    {
        return Integer(min, max) & ~1;
    }

    /// <summary>
    /// 随机奇数
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Odd(int max)
    {
        return Integer(max) | 1;
    }

    /// <summary>
    /// 随机奇数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Odd(int min, int max)
    {
        min = (min + 1) & ~1;
        max |= 1;

        if(min > max)
        {
            return min;
        }

        return Integer(min, max) | 1;
    }

    #endregion
}

#if NET
internal static partial class Randomizer
{
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
    private static Random _random = new();

    public static void InlitializeRandomSeed(int seed)
    {
        _random = new Random(seed);
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
