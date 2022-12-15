namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;
using OneI.Logable.Templating.Properties;

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

        public ILoggerConfiguration With<T>(string name, T value, IPropertyValueRenderer<T>? renderer)
        {
            return _parent.Use(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value, renderer)));
        }

        public ILoggerConfiguration WithSerializable<T>(string name, T value)
            where T : IPropertyValueRenderer<T>
        {
            return _parent.Use(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value, value)));
        }
    }
}
