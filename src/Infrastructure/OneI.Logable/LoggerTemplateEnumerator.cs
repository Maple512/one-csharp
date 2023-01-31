namespace OneI.Logable;

using OneI.Logable.Templates;

public struct LoggerTemplateEnumerator : IEnumerator<TemplateHolder>
{
    public readonly TemplateEnumerator Template;
    public readonly TemplateEnumerator Message;

    public LoggerTemplateEnumerator(TemplateEnumerator template, TemplateEnumerator message)
    {
        Template = template;
        Message = message;
    }

    public bool _isMessageScope;

    public TemplateHolder Current { get; private set; }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if(_isMessageScope)
        {
            _isMessageScope = MessageMove();
            if(_isMessageScope)
            {
                return true;
            }
        }

        if(Template.MoveNext())
        {
            Current = Template.Current;
            if(Current.Name is { Length: > 0 } && Current.Name.Equals(LoggerConstants.Propertys.Message, StringComparison.InvariantCulture))
            {
                _isMessageScope = MessageMove();
                if(_isMessageScope)
                {
                    return true;
                }
            }

            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private bool MessageMove()
    {
        if(Message.MoveNext())
        {
            Current = Message.Current;
            return true;
        }

        return false;
    }

    public void Reset() => throw new NotImplementedException();

    public void Dispose() => throw new NotImplementedException();

    public ReadOnlyMemory<char> GetCurrentText()
    {
        if(_isMessageScope)
        {
            return Message.Text.Slice(Current.Position, Current.Length);
        }

        return Template.Text.Slice(Current.Position, Current.Length);
    }
}
