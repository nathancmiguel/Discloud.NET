using System.Net;

namespace Discloud.Http;

/// <summary>
/// Streams a caller-provided <see cref="Stream"/> without taking ownership of it.
/// </summary>
/// <remarks>
/// <see cref="StreamContent"/> disposes its stream when the request is disposed. Upload methods accept a stream from
/// the caller, who remains responsible for disposing it, so this content only reads from it.
/// </remarks>
internal sealed class NonOwningStreamContent : HttpContent
{
    private readonly Stream _stream;

    public NonOwningStreamContent(Stream stream)
    {
        _stream = stream;
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        => SerializeToStreamAsync(stream, context, CancellationToken.None);

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
        => _stream.CopyToAsync(stream, cancellationToken);

    protected override bool TryComputeLength(out long length)
    {
        if (_stream.CanSeek)
        {
            length = _stream.Length - _stream.Position;
            return true;
        }

        length = 0;
        return false;
    }
}
