namespace OneI.Openable.Connections;

using System.Net;

public sealed class UriEndPoint : EndPoint
{
    public UriEndPoint(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(nameof(uri));

        Uri = uri;
    }

    public Uri Uri { get; }

    public override string ToString() => Uri.ToString();
}
