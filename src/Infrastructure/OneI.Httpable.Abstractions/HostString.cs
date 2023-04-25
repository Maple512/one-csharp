namespace OneI.Httpable.Abstractions;

using System.Globalization;
using Microsoft.Extensions.Primitives;
using OneI.Httpable.Internal;

/// <summary>
/// 表示URI的Host部分。
/// URI可用于构造正确格式化和编码的URI，以便在HTTP标头中使用
/// </summary>
public readonly struct HostString : IEquatable<HostString>
{
    private readonly string _value;

    /// <summary>
    /// 创建一个新的HostString。
    /// 该值应该是Unicode而非PunyCode，并且可能有一个端口。
    /// IPv4和IPv6是允许的。
    /// </summary>
    /// <param name="value"></param>
    public HostString(string value)
    {
        _value = value;
    }

    public HostString(string host, int port)
    {
        ThrowHelper.ThrowIfNull(host);

        if(port < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(port), "The value must be greater than zero.");
        }

        int index;
        scoped var span = host.AsSpan();
        if(span.Contains('[') == false
            && (index = host.IndexOf(':')) >= 0
            && index < span.Length - 1
            && host.IndexOf(':', index + 1) >= 0)
        {
            host = $"[{host}]";
        }

        _value = $"{host}:{port.ToString(CultureInfo.InvariantCulture)}";
    }

    public string Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _value;
    }

    public bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _value.NotNullOrEmpty();
    }

    public string Host
    {
        get
        {
            Parse(_value, out var host, out _);

            return host.ToString();
        }
    }

    public int? Port
    {
        get
        {
            Parse(_value, out _, out var port);

            if(!StringSegment.IsNullOrEmpty(port)
                && int.TryParse(port.AsSpan(), NumberStyles.None, CultureInfo.InvariantCulture, out var p))
            {
                return p;
            }

            return null;
        }
    }

    public string ToUriComponent()
    {
        if(_value.IsNullOrEmpty())
        {
            return string.Empty;
        }

        int i;
        for(i = 0; i < _value.Length; i++)
        {
            if(!HostStringHelper.IsSafeHostStringChar(_value[i]))
            {
                break;
            }
        }

        if(i != _value.Length)
        {
            Parse(_value, out var host, out var port);

            var mapping = new IdnMapping();
            var encoded = mapping.GetAscii(host.Buffer!, host.Offset, host.Length);

            return StringSegment.IsNullOrEmpty(port)
                ? encoded
                : $"{encoded}:{port}";
        }

        return _value;
    }

    public static HostString FromUriComponent(string uriComponent)
    {
        if(uriComponent.NotNullOrEmpty())
        {
            int index;
            if(uriComponent.Contains('['))
            {
                // IPv6 [::1]可能带有端口
            }
            else if((index = uriComponent.IndexOf(':')) >= 0
                && index < uriComponent.Length - 1
                && uriComponent.IndexOf(':', index + 1) >= 0)
            {
                // IPv6 "::1"，是唯一一种具有2个或以上的host类型
            }
            else if(uriComponent.Contains("xn--", StringComparison.Ordinal))
            {
                var mapping = new IdnMapping();
                // punycode
                if(index >= 0)
                {
                    var port = uriComponent.Substring(index);
                    uriComponent = mapping.GetUnicode(uriComponent, 0, index) + port;
                }
                else
                {
                    uriComponent = mapping.GetUnicode(uriComponent);
                }
            }
        }

        return new(uriComponent);
    }

    public static HostString FromUriComponents(Uri uri)
    {
        ThrowHelper.ThrowIfNull(uri);

        return new(uri.GetComponents(UriComponents.NormalizedHost | UriComponents.HostAndPort,
            UriFormat.Unescaped));
    }

    /// <summary>
    /// Host标头的Host部分与模式列表进行匹配。
    /// 只要模式使用相同的格式，Host可以是编码的punycode或解码的unicode形式。
    /// </summary>
    /// <remarks>
    /// 给定值上的端口被忽略。
    /// 模式不应该有端口。模式可以是完全匹配，如：example.com，匹配所有Host的顶级通配符“*”，或“abc.example.com:443”，但不匹配“example.com:444”的子域通配符“*.example.com”。
    /// 匹配不区分大小写。
    /// </remarks>
    /// <param name="value"></param>
    /// <param name="patterns"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static bool MatchesAny(StringSegment value, List<StringSegment> patterns)
    {
        ThrowHelper.ThrowIfNull(value);
        ThrowHelper.ThrowIfNull(patterns);

        Parse(value, out var host, out var port);

        for(int i = 0; i < port.Length; i++)
        {
            var c = port[i];
            if(c < '0'
                || c > '9')
            {
                throw new ArgumentOutOfRangeException($"The given host value '{value}' has a malformed port.");
            }
        }

        for(int i = 0; i < patterns.Count; i++)
        {
            var pattern = patterns[i];

            if(pattern == "*")
            {
                return true;
            }

            if(StringSegment.Equals(pattern, host, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if(pattern.StartsWith("*.", StringComparison.Ordinal)
                && host.Length >= pattern.Length)
            {
                // .example.com
                var allowedRoot = pattern.Subsegment(1);

                var hostRoot = host.Subsegment(host.Length - allowedRoot.Length);

                if(hostRoot.Equals(allowedRoot, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override string ToString()
    {
        return ToUriComponent();
    }

    public bool Equals(HostString other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj)
    {
        return obj is HostString && Equals((HostString)obj);
    }

    public static bool operator ==(HostString left, HostString right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(HostString left, HostString right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    static void Parse(StringSegment value, out StringSegment host, out StringSegment port)
    {
        host = null;
        port = null;
        int index;

        if(StringSegment.IsNullOrEmpty(value))
        {
            return;
        }// IPv6 [::1]
        else if((index = value.IndexOf(']')) >= 0)
        {
            host = value.Subsegment(0, index + 1);

            if(index + 2 < value.Length && value[index + 1] == ':')
            {
                port = value.Subsegment(index + 2);
            }
        }
        else if((index = value.IndexOf(':')) >= 0
            && index < value.Length - 1
            && value.IndexOf(':', index + 1) >= 0)
        {
            host = $"[{value}]";
            port = null;
        }
        else if(index >= 0)
        {
            host = value.Subsegment(0, index);
            port = value.Subsegment(index + 1);
        }
        else
        {
            host = value;
            port = null;
        }
    }
}
