using System.Text;
using System.Text.Json;
using Discloud.Tests.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Discloud.Tests;

public sealed class ClientBehaviorTests
{
    private static readonly JsonSerializerOptions ReflectionJson = new() { TypeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver() };

    private const string UserJson = """{"status":"ok","user":{"userID":"1"}}""";

    [Fact]
    public async Task AddDiscloudClient_reads_token_from_the_Discloud_section_of_appsettings()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes("""{"Discloud":{"Token":"from-json","Timeout":"00:00:30"}}""")))
            .Build();
        var handler = MockHttpHandler.Json(UserJson);
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddDiscloudClient().ConfigurePrimaryHttpMessageHandler(() => handler);

        await using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IDiscloudClient>();
        await client.User.GetAsync(TestContext.Current.CancellationToken);

        Assert.Equal("from-json", handler.LastRequest.Header("api-token"));
        Assert.Equal(TimeSpan.FromSeconds(30), provider.GetRequiredService<IOptions<DiscloudClientOptions>>().Value.Timeout);
    }

    [Fact]
    public async Task AddDiscloudClient_with_section_reads_token_and_base_address()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Discloud:Token"] = "from-section",
                ["Discloud:BaseAddress"] = "https://staging.example.test/v2/",
            })
            .Build();
        var handler = MockHttpHandler.Json(UserJson);
        var services = new ServiceCollection();
        services.AddDiscloudClient(configuration.GetSection("Discloud")).ConfigurePrimaryHttpMessageHandler(() => handler);

        await using var provider = services.BuildServiceProvider();
        await provider.GetRequiredService<IDiscloudClient>().User.GetAsync(TestContext.Current.CancellationToken);

        Assert.Equal("from-section", handler.LastRequest.Header("api-token"));
        Assert.Equal("https://staging.example.test/v2/user", handler.LastRequest.Uri.ToString());
    }

    [Fact]
    public async Task Environment_variable_Discloud__Token_overrides_appsettings()
    {
        var variable = "Discloud__Token";
        var previous = Environment.GetEnvironmentVariable(variable);
        Environment.SetEnvironmentVariable(variable, "from-environment");
        try
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes("""{"Discloud":{"Token":""}}""")))
                .AddEnvironmentVariables()
                .Build();
            var handler = MockHttpHandler.Json(UserJson);
            var services = new ServiceCollection();
            services.AddDiscloudClient(configuration.GetSection("Discloud")).ConfigurePrimaryHttpMessageHandler(() => handler);

            await using var provider = services.BuildServiceProvider();
            await provider.GetRequiredService<IDiscloudClient>().User.GetAsync(TestContext.Current.CancellationToken);

            Assert.Equal("from-environment", handler.LastRequest.Header("api-token"));
        }
        finally
        {
            Environment.SetEnvironmentVariable(variable, previous);
        }
    }

    [Fact]
    public async Task AddDiscloudClient_with_delegate_supports_manual_configuration_and_custom_handlers()
    {
        var handler = MockHttpHandler.Json(UserJson);
        var seen = 0;
        var services = new ServiceCollection();
        services.AddDiscloudClient(o => o.Token = "manual")
            .AddHttpMessageHandler(() => new CountingHandler(() => Interlocked.Increment(ref seen)))
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        await using var provider = services.BuildServiceProvider();
        await provider.GetRequiredService<DiscloudClient>().User.GetAsync(TestContext.Current.CancellationToken);

        Assert.Equal("manual", handler.LastRequest.Header("api-token"));
        Assert.Equal(1, seen);
    }

    [Fact]
    public async Task Missing_token_fails_when_the_client_is_resolved()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddDiscloudClient();

        await using var provider = services.BuildServiceProvider();
        var ex = Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IDiscloudClient>());

        Assert.Contains("Discloud:Token", ex.Message);
    }

    [Fact]
    public void Invalid_configured_timeout_is_reported_clearly()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Discloud:Token"] = "t", ["Discloud:Timeout"] = "soon" })
            .Build();
        var services = new ServiceCollection();
        services.AddDiscloudClient(configuration.GetSection("Discloud"));

        using var provider = services.BuildServiceProvider();
        var ex = Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<IDiscloudClient>());

        Assert.Contains("Discloud:Timeout", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Direct_instantiation_validates_the_token(string? token)
    {
        Assert.Throws<ArgumentException>(() => new DiscloudClient(new DiscloudClientOptions { Token = token }));
    }

    [Fact]
    public void Direct_instantiation_validates_base_address_and_timeout()
    {
        Assert.Throws<ArgumentException>(() => new DiscloudClient(new DiscloudClientOptions { Token = "t", BaseAddress = new Uri("/relative", UriKind.Relative) }));
        Assert.Throws<ArgumentException>(() => new DiscloudClient(new DiscloudClientOptions { Token = "t", BaseAddress = new Uri("ftp://example.test/") }));
        Assert.Throws<ArgumentException>(() => new DiscloudClient(new DiscloudClientOptions { Token = "t", Timeout = TimeSpan.Zero }));
        using var infinite = new DiscloudClient(new DiscloudClientOptions { Token = "t", Timeout = Timeout.InfiniteTimeSpan });
    }

    [Fact]
    public void Token_is_never_exposed_by_options()
    {
        var options = new DiscloudClientOptions { Token = "super-secret" };

        Assert.DoesNotContain("super-secret", options.ToString(), StringComparison.Ordinal);
        Assert.Contains("***", options.ToString(), StringComparison.Ordinal);
        Assert.DoesNotContain("super-secret", JsonSerializer.Serialize(options, ReflectionJson), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Concurrent_requests_do_not_share_request_state()
    {
        var handler = new MockHttpHandler(async (request, ct) =>
        {
            await Task.Yield();
            var id = request.RequestUri!.Segments[^2].TrimEnd('/');
            return MockHttpHandler.JsonResponse("{\"status\":\"ok\",\"apps\":{\"id\":\"" + id + "\"}}");
        });
        using var client = TestClient.Create(handler);

        var ids = Enumerable.Range(0, 100).Select(i => $"app{i}").ToArray();
        var results = await Task.WhenAll(ids.Select(id => client.Apps.GetStatusAsync(id, TestContext.Current.CancellationToken)));

        Assert.Equal(ids, results.Select(r => r.Id));
        Assert.Equal(100, handler.Requests.Count);
        Assert.All(handler.Requests, r => Assert.Equal(TestClient.Token, r.Header("api-token")));
    }

    [Fact]
    public async Task Owned_http_client_is_disposed_with_the_client()
    {
        var client = new DiscloudClient("token");
        client.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => client.User.GetAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Provided_http_client_is_not_disposed()
    {
        var handler = MockHttpHandler.Json(UserJson);
        var httpClient = new HttpClient(handler);
        var client = new DiscloudClient(httpClient, Options.Create(new DiscloudClientOptions { Token = "t" }));

        client.Dispose();

        Assert.False(handler.Disposed);
        using var again = new DiscloudClient(httpClient, Options.Create(new DiscloudClientOptions { Token = "t" }));
        await again.User.GetAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Cancellation_token_reaches_the_http_handler()
    {
        using var cts = new CancellationTokenSource();
        var observed = false;
        var handler = new MockHttpHandler((_, ct) =>
        {
            // Cancelling the caller token must be visible to the handler while the request is in flight.
            cts.Cancel();
            observed = ct.IsCancellationRequested;
            return Task.FromResult(MockHttpHandler.JsonResponse(UserJson));
        });
        using var client = TestClient.Create(handler);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.User.GetAsync(cts.Token));

        Assert.True(observed, "The handler token must be linked to the caller token.");
    }

    [Fact]
    public async Task Invalid_arguments_are_rejected_before_any_request()
    {
        using var client = TestClient.Create(UserJson, out var handler);
        var ct = TestContext.Current.CancellationToken;

        await Assert.ThrowsAnyAsync<ArgumentException>(() => client.Apps.GetAsync(" ", ct));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => client.Apps.GetStatusAsync(null!, ct));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => client.Apps.InstallAptAsync("1", [], ct));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => client.Apps.SetRamAsync("1", 0, ct));
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.Apps.UploadAsync(null!, "a.zip", cancellationToken: ct));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => client.Explorer.ReadFileAsync("1", "", ct));
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.Projects.CreateAsync(null!, ct));
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.EnvironmentVariables.SetForAppAsync("1", null!, ct));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => client.Databases.GetBackupDownloadUrlAsync("d", "b", 15, ct));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => client.Teams.AddMemberAsync("t", "u", default, ct));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => client.User.SetLocaleAsync(default, ct));

        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task Unreadable_upload_stream_is_rejected()
    {
        using var client = TestClient.Create(UserJson, out _);
        var stream = new MemoryStream();
        stream.Dispose();

        await Assert.ThrowsAsync<ArgumentException>(() => client.Apps.UploadAsync(stream, "a.zip", cancellationToken: TestContext.Current.CancellationToken));
    }

    private sealed class CountingHandler(Action onSend) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            onSend();
            return base.SendAsync(request, cancellationToken);
        }
    }
}
