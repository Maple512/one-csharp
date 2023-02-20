namespace OneI.Logable;

using OneI.Logable.Templates;

public struct LoggerTemplateEnumerator : IEnumerator<TemplateHolder>
{
    private int _messageIndex;
    public TemplateHolder[] Template;
    public TemplateEnumerator Message;
    private TemplateHolder _current;

    public LoggerTemplateEnumerator(TemplateHolder[] template, int messageIndex, TemplateEnumerator message)
    {
        _messageIndex = messageIndex;
        Template = template;
        Message = message;
    }

    private byte _index;

    public TemplateHolder Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _current;
        }
    }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if(_messageIndex == _index)
        {
            if(Message.MoveNext())
            {
                _current = Message.Current;
                return true;
            }

            _index++;
        }

        if(_index >= Template.Length)
        {
            return false;
        }

        _current = Template[_index++];

        return true;
    }

    public void Reset() => throw new NotImplementedException();

    public void Dispose() => throw new NotImplementedException();
}
