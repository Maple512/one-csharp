namespace OneI.Logable.Configurations;

public interface ILoggerPropertyConfiguration
{
    /// <summary>
    /// 在<see cref="LoggerContext"/>中添加一个属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ILoggerConfiguration With<T>(string name, T value);

    /// <summary>
    /// 在<see cref="LoggerContext"/>中添加一个属性，并且在输出时使用指定的序列化器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="serializable">指定<typeparamref name="T"/>输出时使用的序列化器</param>
    /// <returns></returns>
    ILoggerConfiguration With<T>(string name, T value, IPropertyValueRenderer<T>? serializable);

    /// <summary>
    /// 在<see cref="LoggerContext"/>中添加一个属性，并且在输出时使用指定的序列化器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ILoggerConfiguration WithSerializable<T>(string name, T value)
        where T : IPropertyValueRenderer<T>;
}
