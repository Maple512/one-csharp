namespace OneI.Logable;

using Configurations;
using OneI.Logable.Templates;

public partial class LoggerConfiguration
{
    private class TemplateConfiguration : ILoggerTemplateConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public TemplateConfiguration(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        public ILoggerConfiguration Default(string template)
        {
            _parent._defaultTemplate = template.AsMemory();

            return _parent;
        }

        public ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, string template)
        {
            Check.NotNull(condition);

            _parent._templateProviders.Add(new TemplateItem(condition, template.AsMemory()));

            return _parent;
        }
    }
}
