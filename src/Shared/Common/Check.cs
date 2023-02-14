namespace OneI;

[DebuggerStepThrough]
internal static partial class Check
{
    public static bool IsIn<T>(T? value, params T[] data)
    {
        if(value is null || data is { Length: 0 })
        {
            return false;
        }

        foreach(var item in data)
        {
            if(value.Equals(item))
            {
                return true;
            }
        }

        return false;
    }

    public static void ThrowNullOrWhiteSpace([NotNull] string? value)
#pragma warning disable CS8777 
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
        }
    }
#pragma warning restore CS8777 

    public static void ThrowIfNull<T>([NotNull] T? value)
    {
        if(value == null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
        }
    }

    public static void ThrowIfNullOrEmpty<T>([NotNull] IEnumerable<T> source)
    {
        if(source is null || source.Any() == false)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
        }
    }
}
