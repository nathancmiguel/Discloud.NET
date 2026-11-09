using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

internal sealed class ExplorerClient(DiscloudConnection connection) : IExplorerClient
{
    private static readonly DiscloudJsonContext Json = DiscloudJsonContext.Default;

    public Task<ExplorerListResponse> ListAsync(string appId, string? path = null, CancellationToken cancellationToken = default)
    {
        var route = ApiPath.WithQuery(ExplorerPath(appId), ("cPath", path));
        return connection.SendAsync(HttpMethod.Get, route, Json.ExplorerListResponse, cancellationToken);
    }

    public Task<ExplorerOperationResponse> CreateAsync(string appId, string path, ExplorerItemType type, CancellationToken cancellationToken = default)
    {
        var route = ExplorerPath(appId);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(type.Value, nameof(type));
        return connection.SendJsonAsync(
            HttpMethod.Post, route, new ExplorerCreateRequest(path, type), Json.ExplorerCreateRequest, Json.ExplorerOperationResponse, cancellationToken);
    }

    public Task<ExplorerOpenResponse> ReadFileAsync(string appId, string path, CancellationToken cancellationToken = default)
    {
        var route = ExplorerPath(appId) + "/open";
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return connection.SendAsync(HttpMethod.Get, ApiPath.WithQuery(route, ("cPath", path)), Json.ExplorerOpenResponse, cancellationToken);
    }

    public Task<ExplorerOperationResponse> WriteFileAsync(string appId, string path, string content, CancellationToken cancellationToken = default)
    {
        var route = ExplorerPath(appId) + "/edit";
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(content);
        return connection.SendJsonAsync(
            HttpMethod.Put, route, new ExplorerEditRequest(path, content), Json.ExplorerEditRequest, Json.ExplorerOperationResponse, cancellationToken);
    }

    private static string ExplorerPath(string appId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(appId);
        return $"app/{ApiPath.Segment(appId)}/explorer";
    }
}
