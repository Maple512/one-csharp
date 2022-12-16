namespace OneI;

using System.Security.Cryptography;

/// <summary>
/// 随机数
/// </summary>
[StackTraceHidden]
[DebuggerStepThrough]
public static class Randomizer
{
    const string character_set = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_-+=[{]};:>|./?";

    /// <summary>
    /// 获取一段随机字符串
    /// </summary>
    /// <returns></returns>
    public static string String(int length)
    {
        Span<char> span = stackalloc char[length];

        for(int i = 0; i < length; i++)
        {
            span[i] = Item<char>(character_set);
        }

        return span.ToString();
    }

    /// <summary>
    /// 获取一段随机字符串
    /// </summary>
    /// <returns></returns>
    public static string String()
    {
        return String(Integer(character_set.Length));
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
}
