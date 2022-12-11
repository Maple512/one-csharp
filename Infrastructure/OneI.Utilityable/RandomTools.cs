namespace OneI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public static class RandomTools
{
#if NET7_0_OR_GREATER
    public static int GetRandom(int minValue, int maxValue) => RandomNumberGenerator.GetInt32(minValue, maxValue);

    public static int GetRandom(int maxValue) => RandomNumberGenerator.GetInt32(maxValue);

    public static Span<byte> GetRandomBytes(int length)
    {
        var bytes = new byte[length];

        RandomNumberGenerator.Fill(bytes);

        return bytes;
    }

    public static T GetRandomOf<T>(params T[] objs)
    {
        CheckTools.NotNullOrEmpty(objs);

        return objs[GetRandom(0, objs.Length)];
    }

    public static T GetRandomOfList<T>(IEnumerable<T> list)
    {
        CheckTools.NotNullOrEmpty(list);

        return list.ElementAt(GetRandom(0, list.Count()));
    }

    public static List<T> GenerateRandomizedList<T>(IEnumerable<T> items)
    {
        CheckTools.NotNull(items);

        var currentList = new List<T>(items);
        var randomList = new List<T>();

        while(currentList.Any())
        {
            var randomIndex = GetRandom(0, currentList.Count);

            randomList.Add(currentList[randomIndex]);

            currentList.RemoveAt(randomIndex);
        }

        return randomList;
    }
#endif
}
