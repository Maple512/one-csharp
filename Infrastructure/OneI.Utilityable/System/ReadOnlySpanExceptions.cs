namespace System;

public static class ReadOnlySpanExceptions
{
    public static bool IsEmptyOrWithSpace(this ReadOnlySpan<byte> span)
    {
        if(span.IsEmpty)
        {
            return false;
        }

        for(var i = 0; i < span.Length; i++)
        {
            if(char.IsWhiteSpace((char)span[i]))
            {
                return false;
            }
        }

        return true;
    }
}
