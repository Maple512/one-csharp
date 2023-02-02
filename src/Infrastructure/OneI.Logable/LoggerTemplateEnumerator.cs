namespace OneI.Logable;

using OneI.Logable.Templates;

public struct LoggerTemplateEnumerator : IEnumerator<TemplateHolder>
{
    private int _messageIndex;
    public readonly TemplateHolder[] Template;
    public TemplateEnumerator Message;

    public LoggerTemplateEnumerator(TemplateHolder[] template, int messageIndex, TemplateEnumerator message)
    {
        _messageIndex = messageIndex;
        Template = template;
        Message = message;
    }

    public bool _isMessageScope;
    private byte _index;

    public TemplateHolder Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get; private set;
    }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if(_messageIndex == _index)
        {
            if(Message.MoveNext())
            {
                Current = Message.Current;
                return true;
            }

            _index++;
        }

        if(_index >= Template.Length)
        {
            return false;
        }

        Current = Template[_index++];

        return true;
    }

    public void Reset() => throw new NotImplementedException();

    public void Dispose() => throw new NotImplementedException();
}
