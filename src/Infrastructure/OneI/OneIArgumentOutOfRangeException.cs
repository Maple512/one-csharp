namespace OneI;

using System;
using System.Numerics;
using System.Runtime.Serialization;

[Serializable]
public class OneIArgumentOutOfRangeException<T> : ArgumentException
{
    private readonly T? _actualValue;

    public OneIArgumentOutOfRangeException()
        : base("Specified argument was out of the range of valid values.")
    {
        HResult = HResults.COR_E_ARGUMENTOUTOFRANGE;
    }

    public OneIArgumentOutOfRangeException(string? paramName)
        : base("Specified argument was out of the range of valid values.", paramName)
    {
        HResult = HResults.COR_E_ARGUMENTOUTOFRANGE;
    }

    public OneIArgumentOutOfRangeException(string? paramName, string? message)
        : base(message, paramName)
    {
        HResult = HResults.COR_E_ARGUMENTOUTOFRANGE;
    }

    public OneIArgumentOutOfRangeException(string? message, Exception? innerException)
        : base(message, innerException)
    {
        HResult = HResults.COR_E_ARGUMENTOUTOFRANGE;
    }

    public OneIArgumentOutOfRangeException(T? actualValue, string? paramName, string? message)
        : base(message, paramName)
    {
        _actualValue = actualValue;
        HResult = HResults.COR_E_ARGUMENTOUTOFRANGE;
    }

    public override string Message
    {
        get
        {
            var s = base.Message;
            if(_actualValue != null)
            {
                var valueMessage = $"Actual value was {_actualValue}.";
                if(s == null)
                {
                    return valueMessage;
                }

                return $"{s}{Environment.NewLine}{valueMessage}";
            }

            return s;
        }
    }

    public virtual T? ActualValue => _actualValue;

    /// <summary>
    /// 等于0
    /// </summary>
    public static void ThrowIfZero<TValue>(TValue value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where TValue : INumberBase<TValue>
    {
        if(TValue.IsZero(value))
        {
            OneIArgumentOutOfRangeException<TValue>.ThrowZero(paramName, value);
        }
    }

    /// <summary>
    /// 小于0
    /// </summary>
    public static void ThrowIfNegative<TValue>(TValue value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where TValue : INumberBase<TValue>
    {
        if(TValue.IsNegative(value))
        {
            OneIArgumentOutOfRangeException<TValue>.ThrowNegative(paramName, value);
        }
    }

    /// <summary>
    /// 小于或等于0
    /// </summary>
    public static void ThrowIfNegativeOrZero<TValue>(TValue value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where TValue : INumberBase<TValue>
    {
        if(TValue.IsNegative(value) || TValue.IsZero(value))
        {
            OneIArgumentOutOfRangeException<TValue>.ThrowNegativeOrZero(paramName, value);
        }
    }

    /// <summary>
    /// 大于<paramref name="other"/>
    /// </summary>
    public static void ThrowIfGreaterThan<TValue>(TValue value, TValue other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where TValue : IComparable<TValue>
    {
        if(value.CompareTo(other) > 0)
        {
            OneIArgumentOutOfRangeException<TValue>.ThrowGreater(paramName, value, other);
        }
    }

    /// <summary>
    /// 大于或等于<paramref name="other"/>
    /// </summary>
    public static void ThrowIfGreaterThanOrEqual<TValue>(TValue value, TValue other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where TValue : IComparable<TValue>
    {
        if(value.CompareTo(other) >= 0)
        {
            OneIArgumentOutOfRangeException<TValue>.ThrowGreaterEqual(paramName, value, other);
        }
    }

    /// <summary>
    /// 小于<paramref name="other"/>
    /// </summary>
    public static void ThrowIfLessThan<TValue>(TValue value, TValue other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where TValue : IComparable<TValue>
    {
        if(value.CompareTo(other) < 0)
        {
            OneIArgumentOutOfRangeException<TValue>.ThrowLess(paramName, value, other);
        }
    }

    /// <summary>
    /// 小于或等于<paramref name="other"/>
    /// </summary>
    public static void ThrowIfLessThanOrEqual<TValue>(TValue value, TValue other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where TValue : IComparable<TValue>
    {
        if(value.CompareTo(other) <= 0)
        {
            OneIArgumentOutOfRangeException<TValue>.ThrowLessEqual(paramName, value, other);
        }
    }

    [DoesNotReturn]
    internal static void ThrowZero(string? paramName, T value)
    {
        throw new OneIArgumentOutOfRangeException<T>(value, paramName, $"'{paramName}' must be a non-zero value.");
    }

    [DoesNotReturn]
    internal static void ThrowNegative(string? paramName, T value)
    {
        throw new OneIArgumentOutOfRangeException<T>(value, paramName, $"'{paramName}' must be a non-negative value.");
    }

    [DoesNotReturn]
    internal static void ThrowNegativeOrZero(string? paramName, T value)
    {
        throw new OneIArgumentOutOfRangeException<T>(value, paramName, $"'{paramName}' must be a non-negative and non-zero value.");
    }

    [DoesNotReturn]
    internal static void ThrowGreater(string? paramName, T value, T other)
    {
        throw new OneIArgumentOutOfRangeException<T>(value, paramName, $"'{paramName}' must be less than or equal to '{other}'.");
    }

    [DoesNotReturn]
    internal static void ThrowGreaterEqual(string? paramName, T value, T other)
    {
        throw new OneIArgumentOutOfRangeException<T>(value, paramName, $"'{paramName}' must be less than '{other}'.");
    }

    [DoesNotReturn]
    internal static void ThrowLess(string? paramName, T value, T other)
    {
        throw new OneIArgumentOutOfRangeException<T>(value, paramName, $"'{paramName}' must be greater than or equal to '{other}'.");
    }

    [DoesNotReturn]
    internal static void ThrowLessEqual(string? paramName, T value, T other)
    {
        throw new OneIArgumentOutOfRangeException<T>(value, paramName, $"'{paramName}' must be greater than '{other}'.");
    }
}
