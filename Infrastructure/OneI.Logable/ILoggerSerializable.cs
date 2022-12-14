namespace OneI.Logable;

/// <summary>
/// 表示日志对象的自定义序列化接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ILoggerSerializable<T>
{
    string Serialize(T value);
}
