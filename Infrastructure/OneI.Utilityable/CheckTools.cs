namespace OneI;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// 检查工具类
/// </summary>
[StackTraceHidden]
[DebuggerStepThrough]
public static class CheckTools
{
    [return: NotNull]
    public static string NotNullOrEmpty(
        string? value,
        [CallerArgumentExpression("value")] string? argumentExpression = null) => string.IsNullOrEmpty(value) ? throw new ArgumentNullException(argumentExpression) : value;

    [return: NotNull]
    public static string NotNullOrWhiteSpace(
        string? value,
        string? errorMsg = null,
        [CallerArgumentExpression("value")] string? argumentExpression = null) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentNullException(argumentExpression, errorMsg) : value;

    [return: NotNull]
    public static T NotNull<T>(
        T? value,
        string? errorMsg = null,
        [CallerArgumentExpression("value")] string? argumentExpression = null) => value == null ? throw new ArgumentNullException(argumentExpression, errorMsg) : value;

    [return: NotNull]
    public static IEnumerable<T> NotNullOrEmpty<T>(
        IEnumerable<T>? data,
        [CallerArgumentExpression("data")] string? argumentExpression = null)
    {
        return data?.Any() != true
            ? throw new ArgumentNullException(argumentExpression)
            : data;
    }

    [return: NotNull]
    public static Dictionary<TKey, TValue> NotNullOrEmpty<TKey, TValue>(
        Dictionary<TKey, TValue>? data,
        [CallerArgumentExpression("data")] string? argumentExpression = null)
        where TKey : notnull
    {
        return data == null || !data.Any()
            ? throw new ArgumentNullException(argumentExpression)
            : data;
    }

    [Conditional("DEBUG")]
    public static void DebugAssert(
        [DoesNotReturnIf(false)] bool condition,
        [CallerArgumentExpression("condition")] string? argumentExpression = null)
    {
        if(!condition)
        {
            throw new ArgumentException($"Check assert failed, expression: {argumentExpression}");
        }
    }

    public static bool IsIn<T>([NotNullWhen(true)] T? item, params T[] data)
    {
        return item != null
            && data.Length != 0
            && data.Contains(item);
    }

    /// <summary>
    /// 验证
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="validateAllProperties"><see langword="true"/>时，检查所有属性，否则，仅检查标注了<see cref="RequiredAttribute"/>的属性，默认<see langword="true"/></param>
    /// <returns></returns>
    /// <exception cref="AggregateException"></exception>
    [return: NotNullIfNotNull("instance")]
    public static T Valid<T>([NotNullWhen(true)] T instance, bool validateAllProperties = true)
        where T : class
    {
        var result = TryValidate(instance, validateAllProperties);

        if(result.IsValid)
        {
            return instance;
        }

        throw new AggregateException(result.Errors.Select(x => new ValidationException(
                x,
                null,
                instance)));
    }

    /// <summary>
    /// 验证
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="validateAllProperties"><see langword="true"/>时，检查所有属性，否则，仅检查标注了<see cref="RequiredAttribute"/>的属性，默认<see langword="true"/></param>
    /// <returns></returns>
    public static ValidationResults TryValidate<T>(T instance, bool validateAllProperties = true)
        where T : class
    {
        NotNull(instance);

        var errors = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            instance!,
            new(instance!, null, null),
            errors,
            validateAllProperties);

        return new ValidationResults(isValid, errors);
    }
}

/// <summary>
/// 验证结果集
/// </summary>
public readonly ref struct ValidationResults
{
    public ValidationResults(bool isValid, ICollection<ValidationResult> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }

    public bool IsValid { get; }

    public ICollection<ValidationResult> Errors { get; }
}
