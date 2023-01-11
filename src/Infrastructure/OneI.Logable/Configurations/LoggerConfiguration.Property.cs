namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;
using OneI.Logable;
using OneI.Logable.Templating.Properties;
/// <summary>
/// The logger configuration.
/// </summary>

public partial class LoggerConfiguration
{
    /// <summary>
    /// The logger property configuration.
    /// </summary>
    private class LoggerPropertyConfiguration : ILoggerPropertyConfiguration
    {
        private readonly LoggerConfiguration _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerPropertyConfiguration"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public LoggerPropertyConfiguration(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Withs the.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration With<T>(string name, T value)
        {
            return _parent.Use(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value)));
        }

        /// <summary>
        /// Withs the.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="renderer">The renderer.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration With<T>(string name, T value, IFormatter<T>? renderer)
        {
            return _parent.Use(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value, renderer)));
        }

        /// <summary>
        /// Withs the formatter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>An ILoggerConfiguration.</returns>
        public ILoggerConfiguration WithFormatter<T>(string name, T value)
            where T : IFormatter<T>
        {
            return _parent.Use(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value, value)));
        }
    }
}
