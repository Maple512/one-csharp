namespace OneI.Httpable.Headers;

using Microsoft.Extensions.Primitives;
using System;
using System.Diagnostics.Contracts;
using System.Globalization;

public static class HeaderUtilities
{
    private const int _qualityValueMaxCharCount = 10; // Little bit more permissive than RFC7231 5.3.1
    private const string QualityName = "q";
    internal const string BytesUnit = "bytes";

    /// <summary>
    /// 设置权重
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static void SetQuality(List<NameValuePair> parameters, double? value)
    {
        var quality = NameValuePair.Find(parameters, QualityName);
        if(value.HasValue)
        {
            if((value < 0)
                    || value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            var qualityString = ((double)value).ToString("0.0##", NumberFormatInfo.InvariantInfo);

            if(quality is not null)
            {
                quality.Value = qualityString;
            }
            else
            {
                parameters.Add(new(QualityName, qualityString));
            }
        }
        else
        {
            if(quality is not null)
            {
                parameters.Remove(quality);
            }
        }
    }

    // Strict and fast RFC9110 12.4.2 Quality value parser(and without memory allocation)
    // See https://tools.ietf.org/html/rfc9110#section-12.4.2
    // Check is made to verify if the value is between 0 and 1 (and it returns False if the check fails).

    /// <summary>
    /// 检查权重值，取值范围：[0,1]，0：表示不可接收，1：表示最优（默认）；最多三位小数
    /// </summary>
    /// <param name="input"></param>
    /// <param name="startIndex"></param>
    /// <param name="quality"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    internal static bool TryParseQuality(StringSegment input, int startIndex, out double quality, out int length)
    {
        quality = 0; length = 0;


        return true;
    }

    internal static int GetNextNonEmptyOrWhitespaceIndex(
        StringSegment input,
        int startIndex,
        bool skipEmptyValues,
        out bool separatorFound)
    {
        Contract.Requires(startIndex <= input.Length); // it's OK if index == value.Length.

        separatorFound = false;
        var current = startIndex + HttpRuleParser.GetWhitespaceLength(input, startIndex);

        if((current == input.Length) || (input[current] != ','))
        {
            return current;
        }

        // If we have a separator, skip the separator and all following whitespaces. If we support
        // empty values, continue until the current character is neither a separator nor a whitespace.
        separatorFound = true;
        current++; // skip delimiter.
        current = current + HttpRuleParser.GetWhitespaceLength(input, current);

        if(skipEmptyValues)
        {
            while((current < input.Length) && (input[current] == ','))
            {
                current++; // skip delimiter.
                current = current + HttpRuleParser.GetWhitespaceLength(input, current);
            }
        }

        return current;
    }

    internal static void ThrowIfReadOnly(bool isReadOnly)
    {
        if(isReadOnly)
        {
            throw new InvalidOperationException("The object cannot be modified because it is read-only.");
        }
    }

    internal static void CheckValidToken(StringSegment value)
    {
        if(StringSegment.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"An empty string is not allowed.");
        }
    }
}
