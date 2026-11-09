using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

internal sealed class AppsClient(DiscloudConnection connection) : AppRoutes(connection, "app"), IAppsClient
{
    public Task<UploadResponse> UploadAsync(Stream zipFile, string fileName, string? projectId = null, CancellationToken cancellationToken = default)
    {
        var content = CreateZipContent(zipFile, fileName, projectId);
        return Connection.SendAsync(HttpMethod.Post, "upload", content, Json.UploadResponse, cancellationToken);
    }

    public Task<App> GetAsync(string appId, CancellationToken cancellationToken = default)
        => GetSingleAsync(appId, string.Empty, Json.AppResponse, r => r.Apps, "apps", cancellationToken);

    public Task<IReadOnlyList<App>> GetAllAsync(CancellationToken cancellationToken = default)
        => GetListAsync(string.Empty, Json.AppResponse, r => r.Apps, cancellationToken);

    public Task<IReadOnlyList<AppStatus>> GetAllStatusesAsync(CancellationToken cancellationToken = default)
        => GetListAsync("status", Json.AppStatusResponse, r => r.Apps, cancellationToken);

    public Task<DiscloudResponse> UpdateProfileAsync(string appId, UpdateAppProfileRequest request, CancellationToken cancellationToken = default)
    {
        var path = Route(ValidId(appId), "profile");
        ArgumentNullException.ThrowIfNull(request);
        return Connection.SendJsonAsync(HttpMethod.Put, path, request, Json.UpdateAppProfileRequest, Json.DiscloudResponse, cancellationToken);
    }

    public Task<DiscloudResponse> InstallAptAsync(string appId, IEnumerable<AptPackage> packages, CancellationToken cancellationToken = default)
        => SendAptAsync(HttpMethod.Put, appId, packages, cancellationToken);

    public Task<DiscloudResponse> UninstallAptAsync(string appId, IEnumerable<AptPackage> packages, CancellationToken cancellationToken = default)
        => SendAptAsync(HttpMethod.Delete, appId, packages, cancellationToken);

    public async Task<AppConsoleResult> RunConsoleCommandAsync(string appId, string command, CancellationToken cancellationToken = default)
    {
        var path = Route(ValidId(appId), "console");
        ArgumentException.ThrowIfNullOrWhiteSpace(command);
        var response = await Connection.SendJsonAsync(
            HttpMethod.Put, path, new AppConsoleRequest(command), Json.AppConsoleRequest, Json.AppConsoleResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.Apps, "apps");
    }

    public Task<ExecResponse> ExecAsync(string appId, string command, CancellationToken cancellationToken = default)
    {
        var path = Route(ValidId(appId), "exec");
        ArgumentException.ThrowIfNullOrWhiteSpace(command);
        return Connection.SendJsonAsync(HttpMethod.Put, path, new ExecRequest(command), Json.ExecRequest, Json.ExecResponse, cancellationToken);
    }

    public Task<AppDeleteResponse> DeleteAsync(string appId, CancellationToken cancellationToken = default)
        => Connection.SendAsync(HttpMethod.Delete, Route(ValidId(appId), "delete"), Json.AppDeleteResponse, cancellationToken);

    public Task<AppDeleteResponse> DeleteAllAsync(CancellationToken cancellationToken = default)
        => Connection.SendAsync(HttpMethod.Delete, Route("all", "delete"), Json.AppDeleteResponse, cancellationToken);

    private Task<DiscloudResponse> SendAptAsync(HttpMethod method, string appId, IEnumerable<AptPackage> packages, CancellationToken cancellationToken)
    {
        var path = Route(ValidId(appId), "apt");
        ArgumentNullException.ThrowIfNull(packages);

        // The API expects one comma-separated string, for example "ffmpeg,java".
        var apt = string.Join(',', packages.Select(p => p.Value).Where(v => v.Length > 0));
        if (apt.Length == 0)
        {
            throw new ArgumentException("At least one APT package is required.", nameof(packages));
        }

        return Connection.SendJsonAsync(method, path, new AppAptRequest(apt), Json.AppAptRequest, Json.DiscloudResponse, cancellationToken);
    }
}
