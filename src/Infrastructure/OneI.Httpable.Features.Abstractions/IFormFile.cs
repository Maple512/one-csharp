namespace OneI.Httpable;

public interface IFormFile
{
    /// <summary>
    /// 获取上载文件的原始内容类型标头
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// 获取上载文件的原始内容处置标头
    /// <para>doc：<see href="https://developer.mozilla.org/zh-CN/docs/web/http/headers/content-disposition"/></para>
    /// </summary>
    string ContentDisposition { get; }

    IHeaderDictionary Headers { get; }

    long Length { get; }

    string Name { get; }

    string FileName { get; }

    Stream OpenReadStream();

    void CopyTo(Stream target);

    Task CopyToAsync(Stream target, CancellationToken cancellationToken = default);
}
