using System.Net.Http.Headers;
using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

/// <summary>
/// The operations shared by <c>/app/{appID}/…</c> (your applications) and <c>/team/{appID}/…</c> (applications you
/// moderate), which use identical request and response shapes.
/// </summary>
internal abstract class AppRoutes(DiscloudConnection connection, string prefix)
{
    private const string AllApps = "all";

    protected static DiscloudJsonContext Json { get; } = DiscloudJsonContext.Default;

    protected DiscloudConnection Connection { get; } = connection;

    public Task<AppStatus> GetStatusAsync(string appId, CancellationToken cancellationToken = default)
        => GetSingleAsync(appId, "status", Json.AppStatusResponse, r => r.Apps, "apps", cancellationToken);

    public Task<AppLogs> GetLogsAsync(string appId, CancellationToken cancellationToken = default)
        => GetSingleAsync(appId, "logs", Json.AppLogsResponse, r => r.Apps, "apps", cancellationToken);

    public Task<IReadOnlyList<AppLogs>> GetAllLogsAsync(CancellationToken cancellationToken = default)
        => GetListAsync("logs", Json.AppLogsResponse, r => r.Apps, cancellationToken);

    public Task<AppBackup> GetBackupAsync(string appId, CancellationToken cancellationToken = default)
        => GetSingleAsync(appId, "backup", Json.AppBackupResponse, r => r.Backups, "backups", cancellationToken);

    public Task<IReadOnlyList<AppBackup>> GetAllBackupsAsync(CancellationToken cancellationToken = default)
        => GetListAsync("backup", Json.AppBackupResponse, r => r.Backups, cancellationToken);

    public Task<AppStartResponse> StartAsync(string appId, CancellationToken cancellationToken = default)
        => Connection.SendAsync(HttpMethod.Put, Route(ValidId(appId), "start"), Json.AppStartResponse, cancellationToken);

    public Task<AppStartResponse> StartAllAsync(CancellationToken cancellationToken = default)
        => Connection.SendAsync(HttpMethod.Put, Route(AllApps, "start"), Json.AppStartResponse, cancellationToken);

    public Task<AppRestartResponse> RestartAsync(string appId, CancellationToken cancellationToken = default)
        => Connection.SendAsync(HttpMethod.Put, Route(ValidId(appId), "restart"), Json.AppRestartResponse, cancellationToken);

    public Task<AppRestartResponse> RestartAllAsync(CancellationToken cancellationToken = default)
        => Connection.SendAsync(HttpMethod.Put, Route(AllApps, "restart"), Json.AppRestartResponse, cancellationToken);

    public Task<AppStopResponse> StopAsync(string appId, CancellationToken cancellationToken = default)
        => Connection.SendAsync(HttpMethod.Put, Route(ValidId(appId), "stop"), Json.AppStopResponse, cancellationToken);

    public Task<AppStopResponse> StopAllAsync(CancellationToken cancellationToken = default)
        => Connection.SendAsync(HttpMethod.Put, Route(AllApps, "stop"), Json.AppStopResponse, cancellationToken);

    public Task<DiscloudResponse> SetRamAsync(string appId, int ramMb, CancellationToken cancellationToken = default)
    {
        var path = Route(ValidId(appId), "ram");
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ramMb);
        return Connection.SendJsonAsync(
            HttpMethod.Put, path, new AppRamUpdateRequest(ramMb), Json.AppRamUpdateRequest, Json.DiscloudResponse, cancellationToken);
    }

    public Task<AppCommitResponse> CommitAsync(string appId, Stream zipFile, string fileName, CancellationToken cancellationToken = default)
    {
        var path = Route(ValidId(appId), "commit");
        var content = CreateZipContent(zipFile, fileName, projectId: null);
        return Connection.SendAsync(HttpMethod.Put, path, content, Json.AppCommitResponse, cancellationToken);
    }

    protected string Route(string appId, string action) => $"{prefix}/{ApiPath.Segment(appId)}/{action}";

    protected static string ValidId(string appId, string paramName = "appId")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(appId, paramName);
        return appId;
    }

    protected async Task<T> GetSingleAsync<TResponse, T>(
        string appId,
        string action,
        System.Text.Json.Serialization.Metadata.JsonTypeInfo<TResponse> responseType,
        Func<TResponse, IReadOnlyList<T>?> select,
        string propertyName,
        CancellationToken cancellationToken)
        where TResponse : DiscloudResponse
    {
        var path = action.Length == 0 ? $"{prefix}/{ApiPath.Segment(ValidId(appId))}" : Route(ValidId(appId), action);
        var response = await Connection.SendAsync(HttpMethod.Get, path, responseType, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.RequireSingle(select(response), propertyName);
    }

    protected async Task<IReadOnlyList<T>> GetListAsync<TResponse, T>(
        string action,
        System.Text.Json.Serialization.Metadata.JsonTypeInfo<TResponse> responseType,
        Func<TResponse, IReadOnlyList<T>?> select,
        CancellationToken cancellationToken)
        where TResponse : DiscloudResponse
    {
        var path = action.Length == 0 ? $"{prefix}/{AllApps}" : Route(AllApps, action);
        var response = await Connection.SendAsync(HttpMethod.Get, path, responseType, cancellationToken).ConfigureAwait(false);
        return select(response) ?? [];
    }

    protected static MultipartFormDataContent CreateZipContent(Stream zipFile, string fileName, string? projectId)
    {
        ArgumentNullException.ThrowIfNull(zipFile);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        if (!zipFile.CanRead)
        {
            throw new ArgumentException("The stream must be readable.", nameof(zipFile));
        }

        var file = new NonOwningStreamContent(zipFile);
        file.Headers.ContentType = new MediaTypeHeaderValue("application/zip");

        var content = new MultipartFormDataContent { { file, "file", fileName } };
        if (projectId is not null)
        {
            content.Add(new StringContent(projectId), "projectId");
        }

        return content;
    }
}
