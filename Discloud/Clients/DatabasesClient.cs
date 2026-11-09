using System.Globalization;
using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

internal sealed class DatabasesClient(DiscloudConnection connection) : IDatabasesClient
{
    private static readonly DiscloudJsonContext Json = DiscloudJsonContext.Default;

    public async Task<IReadOnlyList<DatabasePreset>> ListPresetsAsync(CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, "database/presets", Json.DatabasePresetListResponse, cancellationToken)
            .ConfigureAwait(false);
        return response.Data?.Databases ?? [];
    }

    public async Task<DatabaseNameSuggestions> GetNameSuggestionsAsync(DatabaseEngine? engine = null, CancellationToken cancellationToken = default)
    {
        var path = ApiPath.WithQuery("database/presets/random-names", ("type", engine?.Value));
        var response = await connection.SendAsync(HttpMethod.Get, path, Json.DatabaseNamesResponse, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public async Task<IReadOnlyList<DatabaseSummary>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, "database", Json.DatabaseListResponse, cancellationToken).ConfigureAwait(false);
        return response.Data ?? [];
    }

    public Task<DatabaseCreatedResponse> CreateAsync(CreateDatabaseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return connection.SendJsonAsync(HttpMethod.Post, "database", request, Json.CreateDatabaseRequest, Json.DatabaseCreatedResponse, cancellationToken);
    }

    public async Task<Database> GetAsync(string databaseId, CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, DatabasePath(databaseId), Json.DatabaseResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.Database, "database");
    }

    public Task<DatabaseUpdateResponse> UpdateAsync(string databaseId, UpdateDatabaseRequest request, CancellationToken cancellationToken = default)
    {
        var path = DatabasePath(databaseId);
        ArgumentNullException.ThrowIfNull(request);
        return connection.SendJsonAsync(HttpMethod.Patch, path, request, Json.UpdateDatabaseRequest, Json.DatabaseUpdateResponse, cancellationToken);
    }

    public Task<DiscloudResponse> DeleteAsync(string databaseId, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Delete, DatabasePath(databaseId), Json.DiscloudResponse, cancellationToken);

    public Task<DiscloudResponse> StartAsync(string databaseId, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Put, DatabasePath(databaseId) + "/start", Json.DiscloudResponse, cancellationToken);

    public Task<DiscloudResponse> StopAsync(string databaseId, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Put, DatabasePath(databaseId) + "/stop", Json.DiscloudResponse, cancellationToken);

    public Task<DiscloudResponse> RestartAsync(string databaseId, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Put, DatabasePath(databaseId) + "/restart", Json.DiscloudResponse, cancellationToken);

    public async Task<DatabaseBackup> CreateBackupAsync(string databaseId, bool? gzip = null, CancellationToken cancellationToken = default)
    {
        var path = DatabasePath(databaseId) + "/backup";

        // The body is optional; omit it to let the API apply its default.
        var content = gzip is { } value ? DiscloudConnection.CreateJsonContent(new DatabaseBackupRequest(value), Json.DatabaseBackupRequest) : null;
        var response = await connection.SendAsync(HttpMethod.Post, path, content, Json.DatabaseBackupCreatedResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public async Task<DatabaseBackupPage> ListBackupsAsync(string databaseId, DatabaseBackupListQuery? query = null, CancellationToken cancellationToken = default)
    {
        var path = DatabasePath(databaseId) + "/backups";
        if (query is not null)
        {
            path = ApiPath.WithQuery(
                path,
                ("limit", query.Limit?.ToString(CultureInfo.InvariantCulture)),
                ("cursor", query.Cursor),
                ("detail", query.Detail switch
                {
                    null => null,
                    DatabaseBackupDetail.Full => "full",
                    _ => "minimal",
                }));
        }

        var response = await connection.SendAsync(HttpMethod.Get, path, Json.DatabaseBackupListResponse, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public async Task<DatabaseBackupDownload> GetBackupDownloadUrlAsync(
        string databaseId,
        string backupId,
        int? expiresInDays = null,
        CancellationToken cancellationToken = default)
    {
        var path = DatabasePath(databaseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(backupId);
        if (expiresInDays is { } days)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(days, 1, nameof(expiresInDays));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(days, 14, nameof(expiresInDays));
        }

        path = ApiPath.WithQuery(
            $"{path}/backups/{ApiPath.Segment(backupId)}/download",
            ("expiresInDays", expiresInDays?.ToString(CultureInfo.InvariantCulture)));
        var response = await connection.SendAsync(HttpMethod.Get, path, Json.DatabaseDownloadResponse, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public Task<DatabaseCreatedResponse> RestoreAsync(string databaseId, RestoreDatabaseRequest request, CancellationToken cancellationToken = default)
    {
        var path = DatabasePath(databaseId) + "/restore";
        ArgumentNullException.ThrowIfNull(request);
        return connection.SendJsonAsync(HttpMethod.Post, path, request, Json.RestoreDatabaseRequest, Json.DatabaseCreatedResponse, cancellationToken);
    }

    public async Task<DatabaseLogs> GetLogsAsync(string databaseId, CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, DatabasePath(databaseId) + "/logs", Json.DatabaseLogsResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.App, "app");
    }

    public Task<DiscloudResponse> DeleteLogsAsync(string databaseId, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Delete, DatabasePath(databaseId) + "/logs", Json.DiscloudResponse, cancellationToken);

    private static string DatabasePath(string databaseId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        return $"database/{ApiPath.Segment(databaseId)}";
    }
}
