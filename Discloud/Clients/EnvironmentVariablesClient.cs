using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

internal sealed class EnvironmentVariablesClient(DiscloudConnection connection) : IEnvironmentVariablesClient
{
    private const string AccountPath = "env/account";

    private static readonly DiscloudJsonContext Json = DiscloudJsonContext.Default;

    public Task<IReadOnlyDictionary<string, string>> GetForAccountAsync(CancellationToken cancellationToken = default)
        => GetAsync(AccountPath, cancellationToken);

    public Task<IReadOnlyDictionary<string, string>> SetForAccountAsync(IReadOnlyDictionary<string, string> variables, CancellationToken cancellationToken = default)
        => SetAsync(AccountPath, variables, cancellationToken);

    public Task<int> DeleteForAccountAsync(CancellationToken cancellationToken = default)
        => DeleteAsync(AccountPath, cancellationToken);

    public Task<IReadOnlyDictionary<string, string>> GetForProjectAsync(string projectId, CancellationToken cancellationToken = default)
        => GetAsync(ScopePath("project", projectId, nameof(projectId)), cancellationToken);

    public Task<IReadOnlyDictionary<string, string>> SetForProjectAsync(string projectId, IReadOnlyDictionary<string, string> variables, CancellationToken cancellationToken = default)
        => SetAsync(ScopePath("project", projectId, nameof(projectId)), variables, cancellationToken);

    public Task<int> DeleteForProjectAsync(string projectId, CancellationToken cancellationToken = default)
        => DeleteAsync(ScopePath("project", projectId, nameof(projectId)), cancellationToken);

    public Task<IReadOnlyDictionary<string, string>> GetForAppAsync(string appId, CancellationToken cancellationToken = default)
        => GetAsync(ScopePath("app", appId, nameof(appId)), cancellationToken);

    public Task<IReadOnlyDictionary<string, string>> SetForAppAsync(string appId, IReadOnlyDictionary<string, string> variables, CancellationToken cancellationToken = default)
        => SetAsync(ScopePath("app", appId, nameof(appId)), variables, cancellationToken);

    public Task<int> DeleteForAppAsync(string appId, CancellationToken cancellationToken = default)
        => DeleteAsync(ScopePath("app", appId, nameof(appId)), cancellationToken);

    private async Task<IReadOnlyDictionary<string, string>> GetAsync(string path, CancellationToken cancellationToken)
    {
        var response = await connection.SendAsync(HttpMethod.Get, path, Json.EnvVarsResponse, cancellationToken).ConfigureAwait(false);
        return response.Data?.Vars ?? new Dictionary<string, string>();
    }

    private async Task<IReadOnlyDictionary<string, string>> SetAsync(
        string path,
        IReadOnlyDictionary<string, string> variables,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(variables);
        var response = await connection.SendJsonAsync(
            HttpMethod.Put, path, new EnvVarsRequest(variables), Json.EnvVarsRequest, Json.EnvVarsResponse, cancellationToken).ConfigureAwait(false);
        return response.Data?.Vars ?? new Dictionary<string, string>();
    }

    private async Task<int> DeleteAsync(string path, CancellationToken cancellationToken)
    {
        var response = await connection.SendAsync(HttpMethod.Delete, path, Json.EnvVarsDeleteResponse, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data?.Removed, "data.removed");
    }

    private static string ScopePath(string scope, string id, string paramName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, paramName);
        return $"env/{scope}/{ApiPath.Segment(id)}";
    }
}
