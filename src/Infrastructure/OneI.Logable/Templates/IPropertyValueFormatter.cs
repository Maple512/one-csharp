namespace OneI.Logable.Templates;

/// <summary>
/// 表示指定对象的自定义格式化接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPropertyValueFormatter<T>
{
    void Format(T? value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null);
}
