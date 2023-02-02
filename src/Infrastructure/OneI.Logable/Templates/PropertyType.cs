namespace OneI.Logable.Templates;

public enum PropertyType : sbyte
{
    /// <summary>
    ///     调用对象的<see cref="object.ToString" />方法
    /// </summary>
    None,

    /// <summary>
    ///     使用<c>"</c>包围对象的字符串形式
    /// </summary>
    Stringify,

    /// <summary>
    ///     将解析对象，并以对象的形式表示：<code>{ Name:1 }</code>
    /// </summary>
    Serialize,
}
