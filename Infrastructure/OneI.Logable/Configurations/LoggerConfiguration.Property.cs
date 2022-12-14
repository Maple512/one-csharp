namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;
using OneI.Logable.Templating.Properties;

/// <summary>
/// The logger builder.
/// </summary>
public partial class LoggerConfiguration
{
    /// <summary>
    /// The logger property builder.
    /// </summary>
    private class LoggerPropertyConfiguration : ILoggerPropertyConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerPropertyConfiguration(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// 在<see cref="LoggerContext"/>中添加一个属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ILoggerConfiguration With<T>(string name, T value)
        {
            _parent.Use(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value)));

            return _parent;
        }

        /// <summary>
        /// 在<see cref="LoggerContext"/>中添加一个属性，并且在输出时使用指定的序列化器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="serializable">指定<typeparamref name="T"/>输出时使用的序列化器</param>
        /// <returns></returns>
        public ILoggerConfiguration With<T>(string name, T value, ILoggerSerializable<T>? serializable)
        {
            _parent.Use(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value, serializable)));

            return _parent;
        }

        /// <summary>
        /// 在<see cref="LoggerContext"/>中添加一个属性，并且在输出时使用指定的序列化器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ILoggerConfiguration WithSerializable<T>(string name, T value)
            where T : ILoggerSerializable<T>
        {
            _parent.Use(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value, value)));

            return _parent;
        }
    }
}
