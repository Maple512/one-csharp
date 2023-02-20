namespace OneI.Logable;

using OneI.Logable.Templates;

public partial class LoggerConfiguration
{
    private class TemplateConfiguration : ILoggerTemplateConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public TemplateConfiguration(LoggerConfiguration parent) => _parent = parent;

        public ILoggerConfiguration Default(string template)
        {
            ArgumentException.ThrowIfNullOrEmpty(template);

            _parent._defaultTemplate = new TemplateItem(null, template.AsMemory());

            return _parent;
        }

        public ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, string template)
        {
            ArgumentException.ThrowIfNullOrEmpty(template);

            _parent._templateProviders.Add(new TemplateItem(condition, template.AsMemory()));

            return _parent;
        }
    }
}
