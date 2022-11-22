namespace OneI.Logable.Parsing;

/// <summary>
/// Property 解析类型
/// </summary>
public enum PropertyTokenType
{
    /// <summary>
    /// 强制字符串化，调用属性的 ToString()。(默认）
    /// 属性前缀<c>$</c>，例如：<c>{$UserName}</c>
    /// 或者使用数字，来代表参数的索引，例如：<c>{0}</c>
    /// </summary>
    Stringify,

    /// <summary>
    /// 将对象序列化。
    /// 属性前缀<c>@</c>，例如：<c>{@User}</c>
    /// </summary>
    Deconstruct,
}
