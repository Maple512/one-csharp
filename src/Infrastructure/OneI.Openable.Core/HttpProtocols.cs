namespace OneI.Openable;

[Flags]
public enum HttpProtocols
{
    None = 0,

    Http1 = 1,

    Http2 = 2,

    Http1_2 = Http1 | Http2,

    Http3 = 4,

    Http1_2_3 = Http1 | Http2 | Http3,
}
