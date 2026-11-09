using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

internal sealed class AppModeratorsClient(DiscloudConnection connection) : IAppModeratorsClient
{
    private static readonly DiscloudJsonContext Json = DiscloudJsonContext.Default;

    public async Task<IReadOnlyList<AppModerator>> ListAsync(string appId, CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, TeamPath(appId), Json.AppModeratorListResponse, cancellationToken)
            .ConfigureAwait(false);
        return response.Team ?? [];
    }

    public Task<AppModerator> AddAsync(string appId, string moderatorId, IEnumerable<AppPermission> permissions, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Post, appId, moderatorId, permissions, cancellationToken);

    public Task<AppModerator> UpdateAsync(string appId, string moderatorId, IEnumerable<AppPermission> permissions, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Put, appId, moderatorId, permissions, cancellationToken);

    public Task<DiscloudResponse> RemoveAsync(string appId, string moderatorId, CancellationToken cancellationToken = default)
    {
        var path = TeamPath(appId);
        ArgumentException.ThrowIfNullOrWhiteSpace(moderatorId);
        return connection.SendAsync(HttpMethod.Delete, $"{path}/{ApiPath.Segment(moderatorId)}", Json.DiscloudResponse, cancellationToken);
    }

    private async Task<AppModerator> SendAsync(
        HttpMethod method,
        string appId,
        string moderatorId,
        IEnumerable<AppPermission> permissions,
        CancellationToken cancellationToken)
    {
        var path = TeamPath(appId);
        ArgumentException.ThrowIfNullOrWhiteSpace(moderatorId);
        ArgumentNullException.ThrowIfNull(permissions);

        var request = new AppModeratorRequest(moderatorId, [.. permissions]);
        var response = await connection.SendJsonAsync(method, path, request, Json.AppModeratorRequest, Json.AppModeratorResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.App, "app");
    }

    private static string TeamPath(string appId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(appId);
        return $"app/{ApiPath.Segment(appId)}/team";
    }
}
