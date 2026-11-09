using System.Globalization;
using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

internal sealed class SnapshotsClient(DiscloudConnection connection) : ISnapshotsClient
{
    private static readonly DiscloudJsonContext Json = DiscloudJsonContext.Default;

    public Task<SnapshotListResponse> ListAsync(SnapshotListQuery? query = null, CancellationToken cancellationToken = default)
    {
        var path = query is null
            ? "snapshot"
            : ApiPath.WithQuery(
                "snapshot",
                ("page", query.Page?.ToString(CultureInfo.InvariantCulture)),
                ("limit", query.Limit?.ToString(CultureInfo.InvariantCulture)),
                ("summary", query.Summary is { } summary ? (summary ? "true" : "false") : null),
                ("appID", query.AppId));
        return connection.SendAsync(HttpMethod.Get, path, Json.SnapshotListResponse, cancellationToken);
    }

    public async Task<IReadOnlyList<SnapshotVersion>> ListVersionsAsync(string appId, CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, AppPath(appId), Json.AppSnapshotListResponse, cancellationToken)
            .ConfigureAwait(false);
        return response.Versions ?? [];
    }

    public async Task<CreatedSnapshot> CreateAsync(string appId, CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Post, AppPath(appId), Json.SnapshotCreateResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.Snapshot, "snapshot");
    }

    public async Task<SnapshotDownload> GetDownloadUrlAsync(string appId, string version, CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, VersionPath(appId, version), Json.SnapshotDownloadResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.Download, "download");
    }

    public Task<SnapshotDeleteResponse> DeleteAsync(string appId, string version, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Delete, VersionPath(appId, version), Json.SnapshotDeleteResponse, cancellationToken);

    private static string AppPath(string appId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(appId);
        return $"snapshot/{ApiPath.Segment(appId)}";
    }

    private static string VersionPath(string appId, string version)
    {
        var path = AppPath(appId);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        return $"{path}/versions/{ApiPath.Segment(version)}";
    }
}
