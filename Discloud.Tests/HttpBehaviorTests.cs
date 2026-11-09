using System.Net;
using System.Text.Json.Nodes;
using Discloud.DTOs;
using Discloud.Tests.Infrastructure;

namespace Discloud.Tests;

public sealed class HttpBehaviorTests
{
    private const string Ok = """{"status":"ok"}""";

    [Fact]
    public async Task Requests_carry_token_accept_and_user_agent_headers_only_on_the_request()
    {
        var handler = MockHttpHandler.Json("""{"status":"ok","user":{"userID":"1"}}""");
        var httpClient = new HttpClient(handler);
        using var client = new DiscloudClient(httpClient, Microsoft.Extensions.Options.Options.Create(new DiscloudClientOptions { Token = "abc" }));

        await client.User.GetAsync(TestContext.Current.CancellationToken);

        var request = handler.LastRequest;
        Assert.Equal("abc", request.Header("api-token"));
        Assert.Equal("application/json", request.Header("Accept"));
        Assert.StartsWith("Discloud.Net/", request.Header("User-Agent"));
        Assert.Null(request.Header("Authorization"));
        Assert.Empty(httpClient.DefaultRequestHeaders);
        Assert.Null(httpClient.BaseAddress);
    }

    [Theory]
    [InlineData("https://staging.example.test/v2", "/v2/user")]
    [InlineData("https://staging.example.test/v2/", "/v2/user")]
    [InlineData("http://localhost:5000/", "/user")]
    public async Task Custom_base_address_is_used_with_or_without_trailing_slash(string baseAddress, string expectedPath)
    {
        var handler = MockHttpHandler.Json("""{"status":"ok","user":{}}""");
        using var client = TestClient.Create(handler, o => o.BaseAddress = new Uri(baseAddress));

        await client.User.GetAsync(TestContext.Current.CancellationToken);

        var uri = handler.LastRequest.Uri;
        Assert.Equal(new Uri(baseAddress).Authority, uri.Authority);
        Assert.Equal(expectedPath, uri.AbsolutePath);
    }

    [Theory]
    [InlineData("a/b", "/v2/app/a%2Fb/status")]
    [InlineData("a b", "/v2/app/a%20b/status")]
    [InlineData("../user", "/v2/app/..%2Fuser/status")]
    [InlineData("x?y#z", "/v2/app/x%3Fy%23z/status")]
    public async Task Path_parameters_are_escaped_as_single_segments(string appId, string expected)
    {
        using var client = TestClient.Create("""{"status":"ok","apps":{"id":"1"}}""", out var handler);

        await client.Apps.GetStatusAsync(appId, TestContext.Current.CancellationToken);

        Assert.Equal(expected, handler.LastRequest.Uri.AbsolutePath);
    }

    [Fact]
    public async Task Query_parameters_are_escaped_and_null_values_are_omitted()
    {
        using var client = TestClient.Create("""{"status":"ok","dir":[]}""", out var handler);

        await client.Explorer.ListAsync("1", "/root/my app/ä&b=c", TestContext.Current.CancellationToken);
        Assert.Equal("?cPath=%2Froot%2Fmy%20app%2F%C3%A4%26b%3Dc", handler.LastRequest.Uri.Query);

        await client.Explorer.ListAsync("1", path: null, TestContext.Current.CancellationToken);
        Assert.Equal(string.Empty, handler.LastRequest.Uri.Query);
    }

    [Fact]
    public async Task Snapshot_query_includes_only_set_values()
    {
        using var client = TestClient.Create("""{"status":"ok"}""", out var handler);

        await client.Snapshots.ListAsync(new SnapshotListQuery { Summary = false }, TestContext.Current.CancellationToken);
        Assert.Equal("/v2/snapshot?summary=false", handler.LastRequest.PathAndQuery);

        await client.Snapshots.ListAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("/v2/snapshot", handler.LastRequest.PathAndQuery);
    }

    [Fact]
    public async Task Optional_request_properties_are_omitted_when_null()
    {
        using var client = TestClient.Create(Ok, out var handler);

        await client.Apps.UpdateProfileAsync("1", new UpdateAppProfileRequest { AvatarUrl = "https://x/a.png" }, TestContext.Current.CancellationToken);

        Assert.Equal("""{"avatarURL":"https://x/a.png"}""", handler.LastRequest.Body);
    }

    [Fact]
    public async Task Unlinking_a_project_team_sends_an_explicit_null()
    {
        using var client = TestClient.Create("""{"status":"ok","data":{"_id":"p1","teamID":null}}""", out var handler);

        var project = await client.Projects.SetTeamAsync("p1", teamId: null, TestContext.Current.CancellationToken);

        Assert.Equal("""{"teamID":null}""", handler.LastRequest.Body);
        Assert.Null(project.TeamId);
    }

    [Fact]
    public async Task Database_backup_without_gzip_sends_no_body()
    {
        using var client = TestClient.Create("""{"status":"ok","data":{"backupId":"1"}}""", out var handler);

        await client.Databases.CreateBackupAsync("d1", cancellationToken: TestContext.Current.CancellationToken);

        Assert.Null(handler.LastRequest.Body);
    }

    [Fact]
    public async Task Create_database_serializes_nested_settings_and_vars()
    {
        using var client = TestClient.Create("""{"status":"ok","id":"d1"}""", out var handler);

        await client.Databases.CreateAsync(
            new CreateDatabaseRequest
            {
                DisplayName = "Staging",
                Engine = new DatabaseEngineSettings { Type = DatabaseEngine.Postgres, Version = "17", Extensions = ["pgvector"], Port = 5433 },
                TotalRam = 512,
                Vars = new Dictionary<string, string> { ["POSTGRES_DB"] = "app" },
                ProjectId = "p1",
                AutoRestart = true,
            },
            TestContext.Current.CancellationToken);

        var expected = JsonNode.Parse("""
            {"displayName":"Staging","db":{"type":"postgres","version":"17","extensions":["pgvector"],"port":5433},
             "totalRam":512,"vars":{"POSTGRES_DB":"app"},"projectID":"p1","autoRestart":true}
            """);
        Assert.True(JsonNode.DeepEquals(expected, JsonNode.Parse(handler.LastRequest.Body!)));
    }

    [Fact]
    public async Task Upload_sends_multipart_with_file_and_project_without_disposing_the_stream()
    {
        using var client = TestClient.Create("""{"status":"ok","app":{"id":"1"}}""", out var handler);
        var zip = new MemoryStream([0x50, 0x4B, 0x03, 0x04]);

        await client.Apps.UploadAsync(zip, "my app.zip", projectId: "proj1", TestContext.Current.CancellationToken);

        var request = handler.LastRequest;
        Assert.StartsWith("multipart/form-data", request.ContentType);
        Assert.Contains("name=file", request.Body);
        Assert.Contains("filename=\"my app.zip\"", request.Body);
        Assert.Contains("Content-Type: application/zip", request.Body);
        Assert.Contains("PK", request.Body);
        Assert.Contains("name=projectId", request.Body);
        Assert.Contains("proj1", request.Body);
        Assert.True(zip.CanRead, "The caller's stream must not be disposed.");
    }

    [Fact]
    public async Task Upload_without_project_omits_the_project_part()
    {
        using var client = TestClient.Create("""{"status":"ok","app":{"id":"1"}}""", out var handler);

        await client.Apps.UploadAsync(new MemoryStream([1]), "a.zip", cancellationToken: TestContext.Current.CancellationToken);

        Assert.DoesNotContain("projectId", handler.LastRequest.Body);
    }

    [Fact]
    public async Task Add_app_to_project_returns_link_on_200()
    {
        using var client = TestClient.Create(
            """{"status":"ok","message":"App added","data":{"_id":"123","projectID":"p1","clusterName":"fenrir"}}""", out _);

        var result = await client.Projects.AddAppAsync("p1", "123", cancellationToken: TestContext.Current.CancellationToken);

        Assert.False(result.IsMigrationStarted);
        Assert.Equal("fenrir", result.Link?.ClusterName);
        Assert.Equal("App added", result.Message);
    }

    [Fact]
    public async Task Add_app_to_project_returns_migration_on_202()
    {
        var handler = MockHttpHandler.Json(
            """
            {"status":"accepted","message":"Migration started","code":"MIGRATION_STARTED",
             "data":{"appID":"123","projectID":"p1","from":"fenrir","to":"hela","estimatedSec":300}}
            """,
            HttpStatusCode.Accepted);
        using var client = TestClient.Create(handler);

        var result = await client.Projects.AddAppAsync("p1", "123", migrate: true, TestContext.Current.CancellationToken);

        Assert.Equal("""{"migrate":true}""", handler.LastRequest.Body);
        Assert.True(result.IsMigrationStarted);
        Assert.Null(result.Link);
        Assert.Equal("hela", result.Migration!.To);
        Assert.Equal(300, result.Migration.EstimatedSeconds);
    }
}
