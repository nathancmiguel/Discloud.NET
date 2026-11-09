using Microsoft.Extensions.Options;

namespace Discloud.Tests.Infrastructure;

public static class TestClient
{
    public const string Token = "test-token-value";

    public static DiscloudClient Create(MockHttpHandler handler, Action<DiscloudClientOptions>? configure = null)
    {
        var options = new DiscloudClientOptions { Token = Token };
        configure?.Invoke(options);
        return new DiscloudClient(new HttpClient(handler) { Timeout = options.Timeout }, Options.Create(options));
    }

    public static DiscloudClient Create(string json, out MockHttpHandler handler)
    {
        handler = MockHttpHandler.Json(json);
        return Create(handler);
    }
}
