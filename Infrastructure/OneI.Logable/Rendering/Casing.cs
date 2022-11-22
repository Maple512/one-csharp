namespace OneI.Logable.Rendering;

public static class Casing
{
    /// <summary>
    /// 根据指定的<paramref name="format"/>转换<paramref name="value"/>的大小写形式
    /// </summary>
    /// <param name="value"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string Format(string value, string? format = null)
    {
        if(format != null
            && format.Length == 1)
        {
            if(format[0] == LoggerDefinition.Formatters.Upper)
            {
                return value.ToUpperInvariant();
            }
            else if(format[0] == LoggerDefinition.Formatters.Lower)
            {
                return value.ToLowerInvariant();
            }
        }

        return value;
    }
}
