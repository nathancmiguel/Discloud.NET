using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// A managed database engine offered by Discloud, with suggested defaults (<c>GET /database/presets</c>).
/// </summary>
public sealed record DatabasePreset
{
    /// <summary>The engine.</summary>
    [JsonPropertyName("type")]
    public DatabaseEngine? Type { get; init; }

    /// <summary>The available engine versions.</summary>
    [JsonPropertyName("versions")]
    public IReadOnlyList<string>? Versions { get; init; }

    /// <summary>The available engine extensions (for example <c>pgvector</c>).</summary>
    [JsonPropertyName("extensions")]
    public IReadOnlyList<string>? Extensions { get; init; }

    /// <summary>The default port of the engine.</summary>
    [JsonPropertyName("defaultPort")]
    public int? DefaultPort { get; init; }

    /// <summary>A randomly generated display name suggestion.</summary>
    [JsonPropertyName("suggestedDisplayName")]
    public string? SuggestedDisplayName { get; init; }

    /// <summary>A randomly generated database name suggestion.</summary>
    [JsonPropertyName("suggestedDbName")]
    public string? SuggestedDbName { get; init; }

    /// <summary>A randomly generated user name suggestion.</summary>
    [JsonPropertyName("suggestedUser")]
    public string? SuggestedUser { get; init; }

    /// <summary>A randomly generated password suggestion. Treat it as a secret.</summary>
    [JsonPropertyName("suggestedPassword")]
    public string? SuggestedPassword { get; init; }

    /// <summary>The environment variables the engine expects when creating a database.</summary>
    [JsonPropertyName("vars")]
    public DatabasePresetVariables? Vars { get; init; }
}

/// <summary>
/// The environment variables a database engine expects.
/// </summary>
public sealed record DatabasePresetVariables
{
    /// <summary>The variables that must be sent when creating a database.</summary>
    [JsonPropertyName("required")]
    public IReadOnlyList<string>? Required { get; init; }

    /// <summary>The variables that may be sent when creating a database.</summary>
    [JsonPropertyName("optional")]
    public IReadOnlyList<string>? Optional { get; init; }

    /// <summary>
    /// Maps each variable name to the preset property holding its suggested value
    /// (for example <c>POSTGRES_USER</c> to <c>suggestedUser</c>).
    /// </summary>
    [JsonPropertyName("fromSuggestions")]
    public IReadOnlyDictionary<string, string>? FromSuggestions { get; init; }
}

/// <summary>
/// Freshly generated name suggestions for a new database (<c>GET /database/presets/random-names</c>).
/// </summary>
public sealed record DatabaseNameSuggestions
{
    /// <summary>A display name suggestion.</summary>
    [JsonPropertyName("suggestedDisplayName")]
    public string? SuggestedDisplayName { get; init; }

    /// <summary>A database name suggestion.</summary>
    [JsonPropertyName("suggestedDbName")]
    public string? SuggestedDbName { get; init; }

    /// <summary>A user name suggestion.</summary>
    [JsonPropertyName("suggestedUser")]
    public string? SuggestedUser { get; init; }
}

/// <summary>
/// A managed database as listed by <c>GET /database</c>. Secrets are not included.
/// </summary>
public sealed record DatabaseSummary
{
    /// <summary>The unique identifier of the database.</summary>
    [JsonPropertyName("_id")]
    public string? Id { get; init; }

    /// <summary>The user ID of the owner.</summary>
    [JsonPropertyName("userID")]
    public string? UserId { get; init; }

    /// <summary>The friendly name of the database.</summary>
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    /// <summary>The linked project, or <see langword="null"/>.</summary>
    [JsonPropertyName("projectID")]
    public string? ProjectId { get; init; }

    /// <summary>The engine settings.</summary>
    [JsonPropertyName("db")]
    public DatabaseEngineInfo? Engine { get; init; }

    /// <summary>The cluster hosting the database.</summary>
    [JsonPropertyName("clusterName")]
    public string? ClusterName { get; init; }

    /// <summary>The RAM allocated to the database, in megabytes.</summary>
    [JsonPropertyName("totalRam")]
    public int? TotalRam { get; init; }

    /// <summary>The private hostname of the database.</summary>
    [JsonPropertyName("hostname")]
    public string? Hostname { get; init; }

    /// <summary>When the database was created, as Unix time in milliseconds.</summary>
    [JsonPropertyName("addedAtTimestamp")]
    public long? AddedAtTimestamp { get; init; }
}

/// <summary>
/// The engine settings of an existing database.
/// </summary>
public sealed record DatabaseEngineInfo
{
    /// <summary>The engine.</summary>
    [JsonPropertyName("type")]
    public DatabaseEngine? Type { get; init; }

    /// <summary>The engine version.</summary>
    [JsonPropertyName("version")]
    public string? Version { get; init; }

    /// <summary>The port the engine listens on.</summary>
    [JsonPropertyName("port")]
    public int? Port { get; init; }
}

/// <summary>
/// A managed database with its live state and decrypted variables (<c>GET /database/{dbID}</c>).
/// </summary>
public sealed record Database
{
    /// <summary>The unique identifier of the database.</summary>
    [JsonPropertyName("_id")]
    public string? Id { get; init; }

    /// <summary>The linked project, or <see langword="null"/>.</summary>
    [JsonPropertyName("projectID")]
    public string? ProjectId { get; init; }

    /// <summary>The cluster hosting the database.</summary>
    [JsonPropertyName("clusterName")]
    public string? ClusterName { get; init; }

    /// <summary>The private hostname of the database.</summary>
    [JsonPropertyName("hostname")]
    public string? Hostname { get; init; }

    /// <summary>The friendly name of the database.</summary>
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    /// <summary>The database family (for example <c>postgresql</c>).</summary>
    [JsonPropertyName("dbType")]
    public string? DbType { get; init; }

    /// <summary>The engine (for example <c>postgres</c>).</summary>
    [JsonPropertyName("dbEngine")]
    public DatabaseEngine? DbEngine { get; init; }

    /// <summary>The engine version.</summary>
    [JsonPropertyName("dbVersion")]
    public string? DbVersion { get; init; }

    /// <summary>The enabled engine extensions.</summary>
    [JsonPropertyName("extensions")]
    public IReadOnlyList<string>? Extensions { get; init; }

    /// <summary>The RAM allocated to the database, in megabytes.</summary>
    [JsonPropertyName("totalRam")]
    public int? TotalRam { get; init; }

    /// <summary>The owner of the database.</summary>
    [JsonPropertyName("owner")]
    public DatabaseOwner? Owner { get; init; }

    /// <summary>
    /// Your permissions when you are not the owner, or <see langword="null"/>. The API contract does not document
    /// the shape of this object, so it is exposed as raw JSON.
    /// </summary>
    [JsonPropertyName("perms")]
    public JsonElement? Permissions { get; init; }

    /// <summary>The engine variables (credentials). Treat them as secrets.</summary>
    [JsonPropertyName("vars")]
    public IReadOnlyDictionary<string, string>? Vars { get; init; }

    /// <summary>Whether the database container is online.</summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }

    /// <summary>Whether the database container exists. It is <see langword="false"/> while provisioning.</summary>
    [JsonPropertyName("exists")]
    public bool? Exists { get; init; }

    /// <summary>The RAM currently used by the database, in megabytes.</summary>
    [JsonPropertyName("totalUsedRam")]
    public double? TotalUsedRam { get; init; }

    /// <summary>The time since the last restart, formatted by the API (for example <c>2 minutes</c>).</summary>
    [JsonPropertyName("last_restart")]
    public string? LastRestart { get; init; }

    /// <summary>The size of the data volume, formatted by the API (for example <c>1.2G</c>).</summary>
    [JsonPropertyName("dataVolumeSizeHuman")]
    public string? DataVolumeSizeHuman { get; init; }
}

/// <summary>
/// The owner of a managed database.
/// </summary>
public sealed record DatabaseOwner
{
    /// <summary>The user ID of the owner.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>The owner's username, if known.</summary>
    [JsonPropertyName("username")]
    public string? Username { get; init; }

    /// <summary>The URL of the owner's avatar, if any.</summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; init; }
}

/// <summary>
/// The data used to create a managed database (<c>POST /database</c>).
/// </summary>
/// <remarks>
/// Use <c>IDatabasesClient.ListPresetsAsync</c> to discover the supported engines, versions and the variables each
/// engine requires.
/// </remarks>
public sealed record CreateDatabaseRequest
{
    /// <summary>The friendly name of the database.</summary>
    [JsonPropertyName("displayName")]
    public required string DisplayName { get; init; }

    /// <summary>The engine settings.</summary>
    [JsonPropertyName("db")]
    public required DatabaseEngineSettings Engine { get; init; }

    /// <summary>The RAM to allocate, in megabytes (minimum 100).</summary>
    [JsonPropertyName("totalRam")]
    public required int TotalRam { get; init; }

    /// <summary>The plaintext engine variables (credentials), such as <c>POSTGRES_PASSWORD</c>.</summary>
    [JsonPropertyName("vars")]
    public IReadOnlyDictionary<string, string>? Vars { get; init; }

    /// <summary>
    /// An optional project to link. The RAM is billed to the project owner and project admin access is required.
    /// </summary>
    [JsonPropertyName("projectID")]
    public string? ProjectId { get; init; }

    /// <summary>Whether the database restarts automatically after a failure.</summary>
    [JsonPropertyName("autoRestart")]
    public bool? AutoRestart { get; init; }
}

/// <summary>
/// The engine settings of a new database.
/// </summary>
public sealed record DatabaseEngineSettings
{
    /// <summary>The engine.</summary>
    [JsonPropertyName("type")]
    public required DatabaseEngine Type { get; init; }

    /// <summary>The engine version (for example <c>17</c>).</summary>
    [JsonPropertyName("version")]
    public required string Version { get; init; }

    /// <summary>Optional engine extensions to enable.</summary>
    [JsonPropertyName("extensions")]
    public IReadOnlyList<string>? Extensions { get; init; }

    /// <summary>An optional port. The engine default is used when omitted.</summary>
    [JsonPropertyName("port")]
    public int? Port { get; init; }
}

/// <summary>
/// The fields to change on a managed database (<c>PATCH /database/{dbID}</c>).
/// Leave a property <see langword="null"/> to keep its current value.
/// </summary>
/// <remarks>
/// Changing <see cref="Hostname"/> or <see cref="TotalRam"/> triggers an asynchronous rebuild of the database.
/// </remarks>
public sealed record UpdateDatabaseRequest
{
    /// <summary>The new friendly name.</summary>
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    /// <summary>
    /// The new private hostname (one or two segments of <c>a-z0-9_</c> joined by <c>-</c>, up to 25 characters).
    /// Requires an owner plan above level 3.
    /// </summary>
    [JsonPropertyName("hostname")]
    public string? Hostname { get; init; }

    /// <summary>The new RAM allocation, in megabytes (100 to 16384).</summary>
    [JsonPropertyName("totalRam")]
    public int? TotalRam { get; init; }
}

/// <summary>
/// The data used to create a new database from a backup (<c>POST /database/{dbID}/restore</c>).
/// </summary>
public sealed record RestoreDatabaseRequest
{
    /// <summary>The identifier of the backup to restore.</summary>
    [JsonPropertyName("backupId")]
    public required string BackupId { get; init; }

    /// <summary>The RAM to allocate to the new database, in megabytes (minimum 100).</summary>
    [JsonPropertyName("totalRam")]
    public required int TotalRam { get; init; }

    /// <summary>An optional friendly name for the new database.</summary>
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    /// <summary>An optional project to link.</summary>
    [JsonPropertyName("projectID")]
    public string? ProjectId { get; init; }

    /// <summary>Optional plaintext engine variables. Mutually exclusive with <see cref="VarsEncrypted"/>.</summary>
    [JsonPropertyName("vars")]
    public IReadOnlyDictionary<string, string>? Vars { get; init; }

    /// <summary>Optional encrypted engine variables. Mutually exclusive with <see cref="Vars"/>.</summary>
    [JsonPropertyName("varsEncrypted")]
    public string? VarsEncrypted { get; init; }

    /// <summary>Whether the new database restarts automatically after a failure.</summary>
    [JsonPropertyName("autoRestart")]
    public bool? AutoRestart { get; init; }

    /// <summary>Optional engine settings for the new database.</summary>
    [JsonPropertyName("db")]
    public DatabaseRestoreEngineSettings? Engine { get; init; }
}

/// <summary>
/// The engine settings of a database restored from a backup.
/// </summary>
public sealed record DatabaseRestoreEngineSettings
{
    /// <summary>The engine version to use for the new database.</summary>
    [JsonPropertyName("version")]
    public string? Version { get; init; }
}

/// <summary>
/// The result of an asynchronous create or restore request. Poll <c>IDatabasesClient.GetAsync</c> with
/// <see cref="Id"/> until the database exists and is online.
/// </summary>
public sealed record DatabaseCreatedResponse : DiscloudResponse
{
    /// <summary>The identifier of the new database.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }
}

/// <summary>
/// The result of updating a managed database (<c>PATCH /database/{dbID}</c>).
/// </summary>
public sealed record DatabaseUpdateResponse : DiscloudResponse
{
    /// <summary>Whether the change triggered an asynchronous rebuild of the database.</summary>
    [JsonPropertyName("rebuilding")]
    public bool? Rebuilding { get; init; }

    /// <summary>The updated database fields.</summary>
    [JsonPropertyName("data")]
    public DatabaseUpdateData? Data { get; init; }
}

/// <summary>
/// The updated fields of a managed database.
/// </summary>
public sealed record DatabaseUpdateData
{
    /// <summary>The unique identifier of the database.</summary>
    [JsonPropertyName("_id")]
    public string? Id { get; init; }

    /// <summary>The friendly name of the database.</summary>
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    /// <summary>The RAM allocated to the database, in megabytes.</summary>
    [JsonPropertyName("totalRam")]
    public int? TotalRam { get; init; }

    /// <summary>The private hostname of the database.</summary>
    [JsonPropertyName("hostname")]
    public string? Hostname { get; init; }
}

/// <summary>
/// A backup created by <c>POST /database/{dbID}/backup</c>.
/// </summary>
public sealed record DatabaseBackup
{
    /// <summary>
    /// The backup identifier. The API sends it as a string or a number; it is always exposed as a string.
    /// </summary>
    [JsonPropertyName("backupId")]
    [JsonConverter(typeof(StringOrNumberJsonConverter))]
    public string? BackupId { get; init; }

    /// <summary>The URL from which the backup can be downloaded.</summary>
    [JsonPropertyName("downloadUrl")]
    public string? DownloadUrl { get; init; }
}

/// <summary>
/// The level of detail of <c>GET /database/{dbID}/backups</c>.
/// </summary>
public enum DatabaseBackupDetail
{
    /// <summary>The default, minimal view.</summary>
    Minimal,

    /// <summary>The full view.</summary>
    Full,
}

/// <summary>
/// Paging options for <c>GET /database/{dbID}/backups</c>. Leave a property <see langword="null"/> to use the API default.
/// </summary>
public sealed record DatabaseBackupListQuery
{
    /// <summary>The page size, from 1 to 1000. The API default is 50.</summary>
    public int? Limit { get; init; }

    /// <summary>The continuation token returned as <see cref="DatabaseBackupPage.NextCursor"/> by a previous call.</summary>
    public string? Cursor { get; init; }

    /// <summary>The level of detail. The API default is <see cref="DatabaseBackupDetail.Minimal"/>.</summary>
    public DatabaseBackupDetail? Detail { get; init; }
}

/// <summary>
/// A page of stored database backups.
/// </summary>
public sealed record DatabaseBackupPage
{
    /// <summary>The backups of the page.</summary>
    [JsonPropertyName("backups")]
    public IReadOnlyList<DatabaseBackupEntry>? Backups { get; init; }

    /// <summary>The token to request the next page, or <see langword="null"/> on the last page.</summary>
    [JsonPropertyName("nextCursor")]
    public string? NextCursor { get; init; }
}

/// <summary>
/// A stored database backup.
/// </summary>
public sealed record DatabaseBackupEntry
{
    /// <summary>
    /// The backup identifier. The API sends it as a string or a number; it is always exposed as a string.
    /// </summary>
    [JsonPropertyName("backupId")]
    [JsonConverter(typeof(StringOrNumberJsonConverter))]
    public string? BackupId { get; init; }

    /// <summary>When the backup was created.</summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>The backup size in bytes.</summary>
    [JsonPropertyName("size")]
    public long? Size { get; init; }
}

/// <summary>
/// A presigned download URL for a database backup.
/// </summary>
public sealed record DatabaseBackupDownload
{
    /// <summary>The presigned download URL.</summary>
    [JsonPropertyName("downloadUrl")]
    public string? DownloadUrl { get; init; }

    /// <summary>When <see cref="DownloadUrl"/> expires.</summary>
    [JsonPropertyName("expiresAt")]
    public DateTimeOffset? ExpiresAt { get; init; }
}

/// <summary>
/// The recent container logs of a managed database (<c>GET /database/{dbID}/logs</c>).
/// </summary>
public sealed record DatabaseLogs
{
    /// <summary>The unique identifier of the database.</summary>
    [JsonPropertyName("_id")]
    public string? Id { get; init; }

    /// <summary>The terminal output.</summary>
    [JsonPropertyName("terminal")]
    public Terminal? Terminal { get; init; }
}

internal sealed record DatabasePresetListResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public DatabasePresetListData? Data { get; init; }
}

internal sealed record DatabasePresetListData
{
    [JsonPropertyName("databases")]
    public IReadOnlyList<DatabasePreset>? Databases { get; init; }
}

internal sealed record DatabaseNamesResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public DatabaseNameSuggestions? Data { get; init; }
}

internal sealed record DatabaseListResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public IReadOnlyList<DatabaseSummary>? Data { get; init; }
}

internal sealed record DatabaseResponse : DiscloudResponse
{
    [JsonPropertyName("database")]
    public Database? Database { get; init; }
}

internal sealed record DatabaseBackupCreatedResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public DatabaseBackup? Data { get; init; }
}

internal sealed record DatabaseBackupListResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public DatabaseBackupPage? Data { get; init; }
}

internal sealed record DatabaseDownloadResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public DatabaseBackupDownload? Data { get; init; }
}

internal sealed record DatabaseLogsResponse : DiscloudResponse
{
    [JsonPropertyName("app")]
    public DatabaseLogs? App { get; init; }
}

internal sealed record DatabaseBackupRequest([property: JsonPropertyName("gzip")] bool Gzip);
