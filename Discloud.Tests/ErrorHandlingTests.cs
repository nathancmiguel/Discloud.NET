using System.Net;
using System.Text;
using System.Text.Json;
using Discloud.Tests.Infrastructure;

namespace Discloud.Tests;

public sealed class ErrorHandlingTests
{
    [Theory]
    [InlineData(HttpStatusCode.BadRequest, typeof(DiscloudApiException))]
    [InlineData(HttpStatusCode.Unauthorized, typeof(DiscloudAuthenticationException))]
    [InlineData(HttpStatusCode.Forbidden, typeof(DiscloudForbiddenException))]
    [InlineData(HttpStatusCode.NotFound, typeof(DiscloudNotFoundException))]
    [InlineData(HttpStatusCode.Conflict, typeof(DiscloudConflictException))]
    [InlineData(HttpStatusCode.TooManyRequests, typeof(DiscloudRateLimitException))]
    [InlineData(HttpStatusCode.InternalServerError, typeof(DiscloudApiException))]
    [InlineData(HttpStatusCode.BadGateway, typeof(DiscloudApiException))]
    [InlineData(HttpStatusCode.ServiceUnavailable, typeof(DiscloudApiException))]
    public async Task Error_status_codes_map_to_typed_exceptions(HttpStatusCode status, Type expectedType)
    {
        var handler = MockHttpHandler.Json("""{"status":"error","message":"Something failed"}""", status);
        using var client = TestClient.Create(handler);

        var ex = await Assert.ThrowsAnyAsync<DiscloudApiException>(() => client.Apps.GetAsync("123", TestContext.Current.CancellationToken));

        Assert.IsType(expectedType, ex);
        Assert.Equal(status, ex.StatusCode);
        Assert.Equal("Something failed", ex.ApiMessage);
        Assert.Equal("GET", ex.RequestMethod);
        Assert.Equal("app/123", ex.RequestPath);
        Assert.Contains(((int)status).ToString(System.Globalization.CultureInfo.InvariantCulture), ex.Message);
        Assert.Contains("Something failed", ex.Message);
    }

    [Fact]
    public async Task Error_code_and_data_are_exposed()
    {
        var handler = MockHttpHandler.Json(
            """
            {"status":"error","code":"CLUSTER_MISMATCH","message":"Different cluster",
             "data":{"appCluster":"fenrir","projectCluster":"hela"}}
            """,
            HttpStatusCode.Conflict);
        using var client = TestClient.Create(handler);

        var ex = await Assert.ThrowsAsync<DiscloudConflictException>(
            () => client.Projects.AddAppAsync("p1", "123", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("CLUSTER_MISMATCH", ex.ErrorCode);
        Assert.Equal("hela", ex.ErrorData!.Value.GetProperty("projectCluster").GetString());
        Assert.Contains("[CLUSTER_MISMATCH]", ex.Message);
    }

    [Fact]
    public async Task Numeric_error_codes_are_exposed_as_strings()
    {
        var handler = MockHttpHandler.Json("""{"status":"error","code":409,"message":"Busy"}""", HttpStatusCode.Conflict);
        using var client = TestClient.Create(handler);

        var ex = await Assert.ThrowsAsync<DiscloudConflictException>(() => client.Snapshots.CreateAsync("a", TestContext.Current.CancellationToken));

        Assert.Equal("409", ex.ErrorCode);
    }

    [Fact]
    public async Task Unauthorized_body_without_status_is_parsed()
    {
        var handler = MockHttpHandler.Json("""{"message":"Please provide a valid token"}""", HttpStatusCode.Unauthorized);
        using var client = TestClient.Create(handler);

        var ex = await Assert.ThrowsAsync<DiscloudAuthenticationException>(() => client.User.GetAsync(TestContext.Current.CancellationToken));

        Assert.Equal("Please provide a valid token", ex.ApiMessage);
        Assert.DoesNotContain(TestClient.Token, ex.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Rate_limit_exposes_retry_after()
    {
        var handler = MockHttpHandler.Respond(_ =>
        {
            var response = MockHttpHandler.JsonResponse("""{"status":"error","message":"Slow down"}""", HttpStatusCode.TooManyRequests);
            response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(30));
            return response;
        });
        using var client = TestClient.Create(handler);

        var ex = await Assert.ThrowsAsync<DiscloudRateLimitException>(() => client.Databases.StartAsync("d1", TestContext.Current.CancellationToken));

        Assert.Equal(TimeSpan.FromSeconds(30), ex.RetryAfter);
    }

    [Fact]
    public async Task Non_json_error_body_still_reports_the_status_without_exposing_the_body()
    {
        var handler = MockHttpHandler.Respond(_ => new HttpResponseMessage(HttpStatusCode.BadGateway)
        {
            Content = new StringContent("<html>secret-proxy-page</html>", Encoding.UTF8, "text/html"),
        });
        using var client = TestClient.Create(handler);

        var ex = await Assert.ThrowsAsync<DiscloudApiException>(() => client.User.GetAsync(TestContext.Current.CancellationToken));

        Assert.Equal(HttpStatusCode.BadGateway, ex.StatusCode);
        Assert.Null(ex.ApiMessage);
        Assert.DoesNotContain("secret-proxy-page", ex.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Empty_error_body_reports_the_status()
    {
        var handler = MockHttpHandler.Respond(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new ByteArrayContent([]) });
        using var client = TestClient.Create(handler);

        var ex = await Assert.ThrowsAsync<DiscloudApiException>(() => client.User.GetAsync(TestContext.Current.CancellationToken));

        Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
    }

    [Fact]
    public async Task Success_status_with_error_body_throws_api_exception()
    {
        using var client = TestClient.Create(
            """{"status":"error","statusCode":93,"message":"The discloud.config file was not found within .zip sent."}""", out _);

        var ex = await Assert.ThrowsAsync<DiscloudApiException>(
            () => client.Apps.UploadAsync(new MemoryStream([1]), "a.zip", cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(HttpStatusCode.OK, ex.StatusCode);
        Assert.Equal("93", ex.ErrorCode);
        Assert.Contains("discloud.config", ex.ApiMessage);
    }

    [Fact]
    public async Task Already_running_app_is_reported_as_an_error()
    {
        using var client = TestClient.Create(
            """{"status":"error","message":"Its application is already on.","appStatus":{"online":true}}""", out _);

        var ex = await Assert.ThrowsAsync<DiscloudApiException>(() => client.Apps.StartAsync("123", TestContext.Current.CancellationToken));

        Assert.Equal("Its application is already on.", ex.ApiMessage);
    }

    [Fact]
    public async Task Malformed_json_throws_invalid_response_with_inner_json_exception()
    {
        using var client = TestClient.Create("""{"status":"ok","user":{""", out _);

        var ex = await Assert.ThrowsAsync<DiscloudInvalidResponseException>(() => client.User.GetAsync(TestContext.Current.CancellationToken));

        Assert.IsAssignableFrom<JsonException>(ex.InnerException);
        Assert.Equal(HttpStatusCode.OK, ex.StatusCode);
        Assert.Equal("user", ex.RequestPath);
    }

    [Fact]
    public async Task Unexpected_json_shape_throws_invalid_response()
    {
        using var client = TestClient.Create("""{"status":"ok","user":"not-an-object"}""", out _);

        await Assert.ThrowsAsync<DiscloudInvalidResponseException>(() => client.User.GetAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Html_success_body_throws_invalid_response()
    {
        var handler = MockHttpHandler.Respond(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("<html></html>", Encoding.UTF8, "text/html"),
        });
        using var client = TestClient.Create(handler);

        await Assert.ThrowsAsync<DiscloudInvalidResponseException>(() => client.Apps.GetAllAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Missing_payload_throws_instead_of_returning_a_default()
    {
        using var client = TestClient.Create("""{"status":"ok","message":"done"}""", out _);

        var ex = await Assert.ThrowsAsync<DiscloudInvalidResponseException>(() => client.Databases.GetAsync("d1", TestContext.Current.CancellationToken));

        Assert.Contains("database", ex.Message);
    }

    [Fact]
    public async Task Empty_single_resource_list_throws_instead_of_returning_null()
    {
        using var client = TestClient.Create("""{"status":"ok","apps":[]}""", out _);

        await Assert.ThrowsAsync<DiscloudInvalidResponseException>(() => client.Apps.GetAsync("123", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Empty_success_body_is_accepted_for_outcome_only_operations()
    {
        var handler = MockHttpHandler.Respond(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent([]) });
        using var client = TestClient.Create(handler);

        var response = await client.Databases.DeleteLogsAsync("d1", TestContext.Current.CancellationToken);

        Assert.Null(response.Status);
    }

    [Fact]
    public async Task Empty_success_body_is_rejected_for_operations_returning_data()
    {
        var handler = MockHttpHandler.Respond(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent([]) });
        using var client = TestClient.Create(handler);

        await Assert.ThrowsAsync<DiscloudInvalidResponseException>(() => client.Teams.ListAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Network_failure_throws_network_exception_with_inner_exception()
    {
        var handler = new MockHttpHandler((_, _) => throw new HttpRequestException("Connection refused"));
        using var client = TestClient.Create(handler);

        var ex = await Assert.ThrowsAsync<DiscloudNetworkException>(() => client.User.GetAsync(TestContext.Current.CancellationToken));

        Assert.IsType<HttpRequestException>(ex.InnerException);
        Assert.False(ex.IsTimeout);
        Assert.Equal("user", ex.RequestPath);
    }

    [Fact]
    public async Task Failure_while_reading_the_body_throws_network_exception()
    {
        var handler = MockHttpHandler.Respond(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(new FailingStream()) });
        using var client = TestClient.Create(handler);

        var ex = await Assert.ThrowsAsync<DiscloudNetworkException>(() => client.User.GetAsync(TestContext.Current.CancellationToken));

        Assert.IsType<IOException>(ex.InnerException);
    }

    [Fact]
    public async Task Timeout_throws_network_exception_flagged_as_timeout()
    {
        var handler = new MockHttpHandler(async (_, ct) =>
        {
            await Task.Delay(TimeSpan.FromSeconds(30), ct);
            return MockHttpHandler.JsonResponse("{}");
        });
        using var client = TestClient.Create(handler, o => o.Timeout = TimeSpan.FromMilliseconds(100));

        var ex = await Assert.ThrowsAsync<DiscloudNetworkException>(() => client.User.GetAsync(TestContext.Current.CancellationToken));

        Assert.True(ex.IsTimeout);
    }

    [Fact]
    public async Task Caller_cancellation_propagates_as_operation_canceled()
    {
        var started = new TaskCompletionSource();
        var handler = new MockHttpHandler(async (_, ct) =>
        {
            started.SetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, ct);
            return MockHttpHandler.JsonResponse("{}");
        });
        using var client = TestClient.Create(handler);
        using var cts = new CancellationTokenSource();

        var call = client.Apps.GetAllAsync(cts.Token);
        await started.Task;
        await cts.CancelAsync();

        var ex = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => call);
        Assert.Equal(cts.Token, ex.CancellationToken);
    }

    [Fact]
    public async Task Already_canceled_token_does_not_send_a_request()
    {
        using var client = TestClient.Create("{}", out var handler);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.User.GetAsync(new CancellationToken(canceled: true)));

        Assert.Empty(handler.Requests);
    }

    private sealed class FailingStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => 0; set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => throw new IOException("Connection reset");
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => throw new IOException("Connection reset");
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
