namespace OneI.Logable;

/// <summary>
/// 表示日志对象的自定义渲染接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPropertyValueRenderer<T>
{
    string Render(T value);
}
