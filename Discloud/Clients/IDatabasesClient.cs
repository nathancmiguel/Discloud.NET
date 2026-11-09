using Discloud.DTOs;

namespace Discloud.Clients;

/// <summary>
/// Manages managed databases: provisioning, lifecycle, backups and logs (<c>/database</c>). Available through
/// <see cref="IDiscloudClient.Databases"/>.
/// </summary>
/// <remarks>
/// Creating and restoring databases requires the Diamond plan (level 4) or higher. Both operations are asynchronous:
/// they return the new identifier immediately; poll <see cref="GetAsync"/> until <see cref="Database.Exists"/> and
/// <see cref="Database.Online"/> are <see langword="true"/>. While another operation runs on the same database the
/// API answers with HTTP 429 (<see cref="DiscloudRateLimitException"/>).
/// </remarks>
public interface IDatabasesClient
{
    /// <summary>
    /// Lists the available engines with their versions, default ports, suggested names and the variables each one
    /// requires (<c>GET /database/presets</c>).
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The engine presets.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<DatabasePreset>> ListPresetsAsync(CancellationToken cancellationToken = default);

    /// <summary>Generates new display, database and user name suggestions (<c>GET /database/presets/random-names</c>).</summary>
    /// <param name="engine">An optional engine to tailor the suggestions to.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The suggestions. No password is generated.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request, for example for an unknown engine.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DatabaseNameSuggestions> GetNameSuggestionsAsync(DatabaseEngine? engine = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the databases you own and those linked to projects of your teams (<c>GET /database</c>). Secrets are omitted.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The databases.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<DatabaseSummary>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Starts provisioning a new database (<c>POST /database</c>).</summary>
    /// <param name="request">The database settings.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The identifier of the database being created.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="DiscloudForbiddenException">The plan or RAM quota is insufficient.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request, for example for missing variables or no available cluster (HTTP 503).</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    /// <example>
    /// <code>
    /// var created = await client.Databases.CreateAsync(new CreateDatabaseRequest
    /// {
    ///     DisplayName = "My Postgres",
    ///     Engine = new DatabaseEngineSettings { Type = DatabaseEngine.Postgres, Version = "17" },
    ///     TotalRam = 512,
    ///     Vars = new Dictionary&lt;string, string&gt;
    ///     {
    ///         ["POSTGRES_DB"] = "app",
    ///         ["POSTGRES_USER"] = "app",
    ///         ["POSTGRES_PASSWORD"] = password,
    ///     },
    /// });
    /// </code>
    /// </example>
    Task<DatabaseCreatedResponse> CreateAsync(CreateDatabaseRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets a database with its live state and decrypted variables (<c>GET /database/{dbID}</c>).</summary>
    /// <param name="databaseId">The database identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The database.</returns>
    /// <exception cref="ArgumentException"><paramref name="databaseId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The database does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<Database> GetAsync(string databaseId, CancellationToken cancellationToken = default);

    /// <summary>Changes the name, hostname and/or RAM of a database (<c>PATCH /database/{dbID}</c>).</summary>
    /// <param name="databaseId">The database identifier.</param>
    /// <param name="request">The fields to change. <see langword="null"/> properties are left unchanged.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The updated fields and whether a rebuild was triggered.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="databaseId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudConflictException">The hostname is already taken.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DatabaseUpdateResponse> UpdateAsync(string databaseId, UpdateDatabaseRequest request, CancellationToken cancellationToken = default);

    /// <summary>Permanently deletes a database and its infrastructure (<c>DELETE /database/{dbID}</c>).</summary>
    /// <param name="databaseId">The database identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="databaseId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> DeleteAsync(string databaseId, CancellationToken cancellationToken = default);

    /// <summary>Starts a database container (<c>PUT /database/{dbID}/start</c>).</summary>
    /// <param name="databaseId">The database identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="databaseId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request, for example because the database is already running.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> StartAsync(string databaseId, CancellationToken cancellationToken = default);

    /// <summary>Stops a database container (<c>PUT /database/{dbID}/stop</c>).</summary>
    /// <param name="databaseId">The database identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="databaseId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request, for example because the database is already stopped.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> StopAsync(string databaseId, CancellationToken cancellationToken = default);

    /// <summary>Restarts a database container (<c>PUT /database/{dbID}/restart</c>).</summary>
    /// <param name="databaseId">The database identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="databaseId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> RestartAsync(string databaseId, CancellationToken cancellationToken = default);

    /// <summary>Creates a backup of a database synchronously (<c>POST /database/{dbID}/backup</c>).</summary>
    /// <param name="databaseId">The database identifier.</param>
    /// <param name="gzip">Whether to compress the dump, or <see langword="null"/> for the API default (compressed).</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The backup identifier and download URL.</returns>
    /// <exception cref="ArgumentException"><paramref name="databaseId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request, for example because the database is offline (HTTP 503).</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DatabaseBackup> CreateBackupAsync(string databaseId, bool? gzip = null, CancellationToken cancellationToken = default);

    /// <summary>Lists the stored backups of a database (<c>GET /database/{dbID}/backups</c>).</summary>
    /// <param name="databaseId">The database identifier.</param>
    /// <param name="query">Paging options, or <see langword="null"/> for the API defaults.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>A page of backups and the cursor of the next page.</returns>
    /// <exception cref="ArgumentException"><paramref name="databaseId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DatabaseBackupPage> ListBackupsAsync(string databaseId, DatabaseBackupListQuery? query = null, CancellationToken cancellationToken = default);

    /// <summary>Generates a presigned download URL for a backup (<c>GET /database/{dbID}/backups/{backupId}/download</c>).</summary>
    /// <param name="databaseId">The database identifier.</param>
    /// <param name="backupId">The backup identifier.</param>
    /// <param name="expiresInDays">How long the URL stays valid, from 1 to 14 days, or <see langword="null"/> for the API default.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The download URL and its expiration.</returns>
    /// <exception cref="ArgumentException"><paramref name="databaseId"/> or <paramref name="backupId"/> is null, empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="expiresInDays"/> is outside 1–14.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DatabaseBackupDownload> GetBackupDownloadUrlAsync(string databaseId, string backupId, int? expiresInDays = null, CancellationToken cancellationToken = default);

    /// <summary>Starts creating a new database from a backup (<c>POST /database/{dbID}/restore</c>).</summary>
    /// <param name="databaseId">The identifier of the database that owns the backup.</param>
    /// <param name="request">The backup to restore and the settings of the new database.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The identifier of the database being created.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="databaseId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DatabaseCreatedResponse> RestoreAsync(string databaseId, RestoreDatabaseRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets the recent container logs of a database (<c>GET /database/{dbID}/logs</c>).</summary>
    /// <param name="databaseId">The database identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The logs.</returns>
    /// <exception cref="ArgumentException"><paramref name="databaseId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DatabaseLogs> GetLogsAsync(string databaseId, CancellationToken cancellationToken = default);

    /// <summary>Purges the stored logs of a database (<c>DELETE /database/{dbID}/logs</c>).</summary>
    /// <param name="databaseId">The database identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="databaseId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> DeleteLogsAsync(string databaseId, CancellationToken cancellationToken = default);
}
