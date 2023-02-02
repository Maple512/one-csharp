namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Templates;

public partial class LoggerConfiguration
{
    private class TemplateConfiguration : ILoggerTemplateConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public TemplateConfiguration(LoggerConfiguration parent) => _parent = parent;

        public ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, string template)
        {
            Check.NotNullOrEmpty(template);

            _parent._templateProviders.Add(new TemplateItem(condition, template.AsMemory()));

            return _parent;
        }
    }
}
