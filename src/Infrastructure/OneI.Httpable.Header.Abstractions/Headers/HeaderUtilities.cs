namespace OneI.Httpable.Headers;

using Microsoft.Extensions.Primitives;
using System.Diagnostics.Contracts;

public static class HeaderUtilities
{
    internal static int GetNextNonEmptyOrWhitespaceIndex(
        StringSegment input,
        int startIndex,
        bool skipEmptyValues,
        out bool separatorFound)
    {
        Contract.Requires(startIndex <= input.Length); // it's OK if index == value.Length.

        separatorFound = false;
        var current = startIndex + HttpRuleParser.GetWhitespaceLength(input, startIndex);

        if((current == input.Length) || (input[current] != ','))
        {
            return current;
        }

        // If we have a separator, skip the separator and all following whitespaces. If we support
        // empty values, continue until the current character is neither a separator nor a whitespace.
        separatorFound = true;
        current++; // skip delimiter.
        current = current + HttpRuleParser.GetWhitespaceLength(input, current);

        if(skipEmptyValues)
        {
            while((current < input.Length) && (input[current] == ','))
            {
                current++; // skip delimiter.
                current = current + HttpRuleParser.GetWhitespaceLength(input, current);
            }
        }

        return current;
    }
}
