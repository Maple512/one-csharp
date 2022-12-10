namespace OneI;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// 检查工具类
/// </summary>
[StackTraceHidden]
[DebuggerStepThrough]
public static class CheckTools
{
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrEmpty(
        string? value,
        [CallerArgumentExpression("value")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrEmpty(value)
            ? throw new ArgumentNullException(expression, ErrorMessage(memberName, filePath, line))
            : value;
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrWhiteSpace(
        string? value,
        [CallerArgumentExpression("value")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentNullException(expression, ErrorMessage(memberName, filePath, line))
            : value;
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(
        T? value,
        [CallerArgumentExpression("value")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
    {
        return value==null
            ? throw new ArgumentNullException(expression, ErrorMessage(memberName, filePath, line))
            : value;
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> NotNullOrEmpty<T>(
        IEnumerable<T>? data,
        [CallerArgumentExpression("data")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
    {
        if (data?.Any()!=true)
        {
            throw new ArgumentNullException(expression, ErrorMessage(memberName, filePath, line));
        }
        else
        {
            return data;
        }
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> NotNullOrEmpty<TKey, TValue>(
        Dictionary<TKey, TValue>? data,
        [CallerArgumentExpression("data")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
        where TKey : notnull
    {
        return data==null||!data.Any()
            ? throw new ArgumentNullException(expression, ErrorMessage(memberName, filePath, line))
            : data;
    }

    public static bool IsIn<T>([NotNullWhen(true)] T? value, params T[] data)
    {
        if (value is null
            ||data is { Length: 0 })
        {
            return false;
        }

        foreach (var item in data)
        {
            if (value.Equals(item))
            {
                return true;
            }
        }

        return false;
    }

    #region Validation

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

        if (result.IsValid)
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

    #endregion Validation

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ErrorMessage(
        string? memberName,
        string? filePath,
        int? lineNumber)
    {
        return $"Value be not null. (\"{filePath}\" L{lineNumber} \"{memberName}\")";
    }
}

/// <summary>
/// 验证结果集
/// </summary>
public readonly ref struct ValidationResults
{
    public ValidationResults(bool isValid, ICollection<ValidationResult> errors)
    {
        IsValid=isValid;
        Errors=errors;
    }

    public bool IsValid { get; }

    public ICollection<ValidationResult> Errors { get; }
}
