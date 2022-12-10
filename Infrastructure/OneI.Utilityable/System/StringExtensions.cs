namespace System;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

[StackTraceHidden]
[DebuggerStepThrough]
public static class StringExtensions
{
    #region Check

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? str) => string.IsNullOrEmpty(str);

    public static bool NotNullOrEmpty([NotNullWhen(true)] this string? str) => string.IsNullOrEmpty(str) == false;

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? str) => string.IsNullOrWhiteSpace(str);

    public static bool NotNullOrWhiteSpace([NotNullWhen(true)] this string? str) => string.IsNullOrWhiteSpace(str) == false;

    #endregion

    #region char case

    /// <summary>
    /// 转驼峰命名（首字符转小写，其余不变）
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>一般情况，首字符转小写，其余不变：<c>HelloWorld</c> -&gt; <c>helloWorld</c></item>
    /// <item>全是大写时，所有字幕均转小写：<c>HELLOWORLD</c> -&gt; <c>helloworld</c></item>
    /// <item>首字符为小写时，全部字符不变：<c>hELLOWORLD</c> -&gt; <c>hELLOWORLD</c></item>
    /// </list>
    /// </remarks>
    /// <param name="str"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull("str")]
    public static string? ToCamelCase(this string? str)
    {
        if(str.IsNullOrWhiteSpace() || !char.IsUpper(str![0]))
        {
            return str;
        }

        var array = str.ToCharArray();

        for(var i = 0; i < array.Length && (i != 1 || char.IsUpper(array[i])); i++)
        {
            var flag = i + 1 < array.Length;

            if(i > 0 && flag && !char.IsUpper(array[i + 1]))
            {
                break;
            }

            array[i] = char.ToLowerInvariant(array[i]);
        }

        return new string(array);
    }

    /// <summary>
    /// 转蛇形命名（大写字符转小写，同时在小写字符和大写字符中间加下划线） 
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>一般情况，大写转小写，同时在小写字符和大写字符中间加下划线：<c>HelloWorld</c> -&gt; <c>hello_world</c></item>
    /// <item>全是大写时，所有字符均转小写：<c>HELLOWORLD</c> -&gt; <c>helloworld</c></item>
    /// </list>
    /// </remarks>
    /// <param name="str"></param>
    /// <param name="separator">分隔符，默认下滑线</param>
    /// <returns></returns>
    [return: NotNullIfNotNull("str")]
    public static string? ToSnakeCase(this string? str, char separator = '_')
    {
        if(str.IsNullOrWhiteSpace())
        {
            return str;
        }

        // * 2：冗余空间
        var stringBuilder = new StringBuilder(str!.Length * 2);

        var snakeCaseState = SnakeCaseState.Start;

        for(var i = 0; i < str.Length; i++)
        {
            if(str[i] == ' ')
            {
                if(snakeCaseState != 0)
                {
                    snakeCaseState = SnakeCaseState.NewWord;
                }
            }
            else if(char.IsUpper(str[i]))
            {
                switch(snakeCaseState)
                {
                    case SnakeCaseState.Upper:
                    {
                        var flag = i + 1 < str.Length;
                        if(i > 0 && flag)
                        {
                            var c = str[i + 1];
                            if(!char.IsUpper(c) && c != separator)
                            {
                                stringBuilder.Append(separator);
                            }
                        }

                        break;
                    }
                    case SnakeCaseState.Lower:
                    case SnakeCaseState.NewWord:
                        stringBuilder.Append(separator);
                        break;
                }

                var value = char.ToLowerInvariant(str[i]);
                stringBuilder.Append(value);
                snakeCaseState = SnakeCaseState.Upper;
            }
            else if(str[i] == separator)
            {
                stringBuilder.Append(separator);
                snakeCaseState = SnakeCaseState.Start;
            }
            else
            {
                if(snakeCaseState == SnakeCaseState.NewWord)
                {
                    stringBuilder.Append(separator);
                }

                stringBuilder.Append(str[i]);
                snakeCaseState = SnakeCaseState.Lower;
            }
        }

        return stringBuilder.ToString();
    }

    #endregion char case

    private enum SnakeCaseState
    {
        Start,
        Lower,
        Upper,
        NewWord,
    }
}
