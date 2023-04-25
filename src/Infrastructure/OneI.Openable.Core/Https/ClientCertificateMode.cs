namespace OneI.Openable.Https;

/// <summary>
/// 客户端证书模式
/// </summary>
public enum ClientCertificateMode
{
    /// <summary>
    /// 不需要客户端证书，不会从客户端请求
    /// </summary>
    No,

    /// <summary>
    /// 将请求客户端，但如果客户端未提供，身份验证不会失败
    /// </summary>
    Allow,

    /// <summary>
    /// 将请求客户端，并且客户端必须提供有效的证书才能进行身份验证
    /// </summary>
    Require,

    /// <summary>
    /// 不需要客户端证书，并且不会在连接开始时请求客户端。应用程序稍后可能会请求它。
    /// </summary>
    Delay,
}
