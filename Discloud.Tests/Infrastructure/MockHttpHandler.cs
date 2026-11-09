using System.Collections.Concurrent;
using System.Net;
using System.Text;

namespace Discloud.Tests.Infrastructure;

/// <summary>
/// A request observed by <see cref="MockHttpHandler"/>, with its body already read.
/// </summary>
public sealed record CapturedRequest(
    HttpMethod Method,
    Uri Uri,
    IReadOnlyDictionary<string, string[]> Headers,
    string? ContentType,
    string? Body)
{
    public string PathAndQuery => Uri.PathAndQuery;

    public string? Header(string name) => Headers.TryGetValue(name, out var values) ? string.Join(",", values) : null;
}

/// <summary>
/// An <see cref="HttpMessageHandler"/> that records requests and answers with a scripted response.
/// No request ever leaves the process.
/// </summary>
public sealed class MockHttpHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _respond;

    public MockHttpHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond)
    {
        _respond = respond;
    }

    public ConcurrentQueue<CapturedRequest> Requests { get; } = new();

    public CapturedRequest LastRequest => Requests.Last();

    public bool Disposed { get; private set; }

    public static MockHttpHandler Json(string json, HttpStatusCode status = HttpStatusCode.OK)
        => new((_, _) => Task.FromResult(JsonResponse(json, status)));

    public static MockHttpHandler Respond(Func<HttpRequestMessage, HttpResponseMessage> respond)
        => new((request, _) => Task.FromResult(respond(request)));

    public static HttpResponseMessage JsonResponse(string json, HttpStatusCode status = HttpStatusCode.OK)
        => new(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        var headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToArray(), StringComparer.OrdinalIgnoreCase);
        Requests.Enqueue(new CapturedRequest(request.Method, request.RequestUri!, headers, request.Content?.Headers.ContentType?.ToString(), body));
        return await _respond(request, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        Disposed = true;
        base.Dispose(disposing);
    }
}
