namespace OneI.Logable;

/// <summary>
/// 表示指定对象的自定义格式化接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IFormatter<T>
{
    /// <summary>
    /// Formats the.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    void Format(T? value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null);
}
