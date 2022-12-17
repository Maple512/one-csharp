namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Formatting;
using OneI.Logable.Middlewares;
using OneI.Textable;
using OneI.Textable.Templating.Properties;

public partial class LoggerConfiguration
{
    private class LoggerPropertyConfiguration : ILoggerPropertyConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerPropertyConfiguration(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        public ILoggerConfiguration With<T>(string name, T value)
        {
            return _parent.Use(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value)));
        }

        public ILoggerConfiguration With<T>(string name, T value, IFormatter<T>? renderer)
        {
            return _parent.Use(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value, renderer)));
        }

        public ILoggerConfiguration WithSerializable<T>(string name, T value)
            where T : IFormatter<T>
        {
            return _parent.Use(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value, value)));
        }
    }
}
