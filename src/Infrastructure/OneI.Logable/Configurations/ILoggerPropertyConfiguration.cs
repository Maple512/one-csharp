namespace OneI.Logable.Configurations;

public interface ILoggerPropertyConfiguration
{
    /// <summary>
    ///  添加一个指定名称的属性，如果队列中已存在，则保留队列中已存在的属性，放弃指定的属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ILoggerConfiguration Add<T>(string name, T value);

    /// <summary>
    /// 添加一个指定名称的属性，如果队列中已存在，则更新，反之，插入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ILoggerConfiguration AddOrUpdate<T>(string name, T value);
}
