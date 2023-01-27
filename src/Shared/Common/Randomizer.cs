namespace OneI;

#if NET
using System.Security.Cryptography;
using System.Text;
#endif

/// <summary>
/// 随机数
/// </summary>
//[DebuggerStepThrough]
internal static partial class Randomizer
{
    /// <summary>
    /// The character_set.
    /// </summary>
    private const string character_set = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_-+=[{]};:>|./?";

    /// <summary>
    /// 获取一段随机字符串
    /// </summary>
    /// <returns></returns>
    public static string String()
    {
        return String(Integer(character_set.Length));
    }

    /// <summary>
    /// 从一个指定数组中随机获取一个子项
    /// </summary>
    public static T Item<T>(params T[] objs)
    {
        Check.NotNullOrEmpty(objs);

        return objs[Integer(objs.Length)];
    }

    /// <summary>
    /// 从一个指定数组中随机获取一个子项
    /// </summary>
    public static T Item<T>(IEnumerable<T> list)
    {
        Check.NotNullOrEmpty(list);

        return list.ElementAt(Integer(list.Count()));
    }

    /// <summary>
    /// 将一个集合顺序随机打乱
    /// </summary>
    public static List<T> Disorder<T>(IEnumerable<T> items)
    {
        Check.NotNull(items);

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

    /// <summary>
    /// 获取随机中文字符
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

            builder.Append(result);
        }

        return builder.ToString();
    }
}

#if NET
[StackTraceHidden]
internal static partial class Randomizer
{
    /// <summary>
    /// 获取一段随机字符串
    /// </summary>
    /// <returns></returns>
    public static string String(int length)
    {
        scoped Span<char> span = stackalloc char[length];

        for(var i = 0; i < length; i++)
        {
            span[i] = Item<char>(character_set);
        }

        return span.ToString();
    }

    /// <summary>
    /// 获取一个指定区间 <c>[min,max)</c> 之间的随机整数
    /// </summary>
    public static int Integer(int minValue, int maxValue)
    {
        return RandomNumberGenerator.GetInt32(minValue, maxValue);
    }

    /// <summary>
    /// 获取一个指定区间 <c>[0,max)</c> 之间的随机整数
    /// </summary>
    public static int Integer(int maxValue)
    {
        return RandomNumberGenerator.GetInt32(maxValue);
    }
}

#elif NETSTANDARD2_0_OR_GREATER
internal static partial class Randomizer
{
    private static readonly Random _random = new();

    /// <summary>
    /// 获取一段随机字符串
    /// </summary>
    /// <returns></returns>
    public static string String(int length)
    {
        var span = new StringBuilder(length);

        for(var i = 0; i < length; i++)
        {
            span.Append(Item<char>(character_set));
        }

        return span.ToString();
    }

    /// <summary>
    /// 获取一个指定区间 <c>[min,max)</c> 之间的随机整数
    /// </summary>
    public static int Integer(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }

    /// <summary>
    /// 获取一个指定区间 <c>[0,max)</c> 之间的随机整数
    /// </summary>
    public static int Integer(int maxValue)
    {
        return _random.Next(maxValue);
    }
}
#endif
