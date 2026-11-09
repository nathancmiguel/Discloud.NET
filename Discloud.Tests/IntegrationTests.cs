namespace Discloud.Tests;

/// <summary>
/// Read-only calls against the real Discloud API. They are skipped unless the <c>DISCLOUD_TOKEN</c> environment
/// variable is set, so ordinary CI runs never contact the API:
/// <code>DISCLOUD_TOKEN=... dotnet test --project Discloud.Tests -- --filter-trait "Category=Integration"</code>
/// </summary>
[Trait("Category", "Integration")]
public sealed class IntegrationTests
{
    private static readonly string? Token = Environment.GetEnvironmentVariable("DISCLOUD_TOKEN");

    [Fact]
    public async Task Get_current_user()
    {
        Assert.SkipWhen(string.IsNullOrWhiteSpace(Token), "Set DISCLOUD_TOKEN to run integration tests.");
        using var client = new DiscloudClient(Token!);

        var user = await client.User.GetAsync(TestContext.Current.CancellationToken);

        Assert.False(string.IsNullOrEmpty(user.UserId));
    }

    [Fact]
    public async Task List_all_apps()
    {
        Assert.SkipWhen(string.IsNullOrWhiteSpace(Token), "Set DISCLOUD_TOKEN to run integration tests.");
        using var client = new DiscloudClient(Token!);

        var apps = await client.Apps.GetAllAsync(TestContext.Current.CancellationToken);

        Assert.NotNull(apps);
    }

    [Fact]
    public async Task Invalid_token_is_rejected()
    {
        Assert.SkipWhen(string.IsNullOrWhiteSpace(Token), "Set DISCLOUD_TOKEN to run integration tests.");
        using var client = new DiscloudClient("invalid-token");

        await Assert.ThrowsAsync<DiscloudAuthenticationException>(() => client.User.GetAsync(TestContext.Current.CancellationToken));
    }
}
