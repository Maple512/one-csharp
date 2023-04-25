namespace OneI.Httpable;

using Microsoft.Extensions.Primitives;

public interface IHeaderDictionary : IDictionary<string, StringValues>
{
    new StringValues this[string key] { get; set; }

    long? ContentLength { get; set; }
}
