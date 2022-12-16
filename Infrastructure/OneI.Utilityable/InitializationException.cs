namespace OneI;

/// <summary>
/// 初始化异常
/// </summary>
[Serializable]
public class InitializationException : Exception
{
    public InitializationException(CalledLocation location)
        : base($"The initialize method can only be called once. First called at: {location}.")
    {
    }
}
