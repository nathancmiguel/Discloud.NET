using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// The kind of a Discloud application. Sent by the API as an integer.
/// </summary>
public enum AppType
{
    /// <summary>A bot or worker application (<c>0</c>).</summary>
    Bot = 0,

    /// <summary>A website or HTTP application (<c>1</c>).</summary>
    Site = 1,
}

/// <summary>
/// An application hosted on Discloud (<c>GET /app/{appID}</c>).
/// </summary>
public sealed record App
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>The URL of the application avatar.</summary>
    [JsonPropertyName("avatarURL")]
    public string? AvatarUrl { get; init; }

    /// <summary>The name of the application.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>Whether the application is a bot or a site.</summary>
    [JsonPropertyName("type")]
    public AppType? Type { get; init; }

    /// <summary>Whether the application is currently online.</summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }

    /// <summary>Whether the application was terminated for exceeding its RAM.</summary>
    [JsonPropertyName("ramKilled")]
    public bool? RamKilled { get; init; }

    /// <summary>The exit code of the application process, if it stopped.</summary>
    [JsonPropertyName("exitCode")]
    public int? ExitCode { get; init; }

    /// <summary>The RAM allocated to the application, in megabytes.</summary>
    [JsonPropertyName("ram")]
    public int? Ram { get; init; }

    /// <summary>The main file of the application.</summary>
    [JsonPropertyName("mainFile")]
    public string? MainFile { get; init; }

    /// <summary>The programming language of the application.</summary>
    [JsonPropertyName("lang")]
    public string? Language { get; init; }

    /// <summary>The Git auto-deployment setting, as sent by the API (for example <c>no</c>).</summary>
    [JsonPropertyName("autoDeployGit")]
    public string? AutoDeployGit { get; init; }

    /// <summary>Whether the application restarts automatically after a failure.</summary>
    [JsonPropertyName("autoRestart")]
    public bool? AutoRestart { get; init; }

    /// <summary>The user IDs of the application moderators.</summary>
    [JsonPropertyName("mods")]
    public IReadOnlyList<string>? Mods { get; init; }

    /// <summary>The APT packages installed on the application.</summary>
    [JsonPropertyName("apts")]
    [JsonConverter(typeof(StringOrStringArrayJsonConverter))]
    public IReadOnlyList<string>? Apts { get; init; }

    /// <summary>When the application was uploaded, as Unix time in milliseconds.</summary>
    [JsonPropertyName("addedTimestamp")]
    public long? AddedTimestamp { get; init; }

    /// <summary>Same as <see cref="AddedTimestamp"/>; some responses use this name instead.</summary>
    [JsonPropertyName("addedAtTimestamp")]
    public long? AddedAtTimestamp { get; init; }

    /// <summary>The cluster hosting the application.</summary>
    [JsonPropertyName("clusterName")]
    public string? ClusterName { get; init; }

    /// <summary>The Git synchronization settings, when the application is linked to a repository.</summary>
    [JsonPropertyName("syncGit")]
    public SyncGit? SyncGit { get; init; }
}

/// <summary>
/// The application created by <c>POST /upload</c>.
/// </summary>
public sealed record UploadedApp
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>Whether the application is a bot or a site.</summary>
    [JsonPropertyName("type")]
    public AppType? Type { get; init; }

    /// <summary>The name of the application.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>The URL of the application avatar.</summary>
    [JsonPropertyName("avatarURL")]
    public string? AvatarUrl { get; init; }

    /// <summary>The RAM allocated to the application, in megabytes.</summary>
    [JsonPropertyName("ram")]
    public int? Ram { get; init; }

    /// <summary>The main file of the application.</summary>
    [JsonPropertyName("mainFile")]
    public string? MainFile { get; init; }

    /// <summary>The programming language of the application.</summary>
    [JsonPropertyName("lang")]
    public string? Language { get; init; }

    /// <summary>The <c>VERSION</c> set in <c>discloud.config</c>.</summary>
    [JsonPropertyName("version")]
    public string? Version { get; init; }

    /// <summary>When the application was uploaded, as Unix time in milliseconds.</summary>
    [JsonPropertyName("addedAtTimestamp")]
    public long? AddedAtTimestamp { get; init; }

    /// <summary>Whether the application restarts automatically after a failure.</summary>
    [JsonPropertyName("autoRestart")]
    public bool? AutoRestart { get; init; }

    /// <summary>
    /// The APT packages installed on the application. The API sends an empty string when there are none;
    /// that is exposed as an empty list.
    /// </summary>
    [JsonPropertyName("apts")]
    [JsonConverter(typeof(StringOrStringArrayJsonConverter))]
    public IReadOnlyList<string>? Apts { get; init; }

    /// <summary>The cluster hosting the application.</summary>
    [JsonPropertyName("clusterName")]
    public string? ClusterName { get; init; }

    /// <summary>The exit code of the application process, if it stopped.</summary>
    [JsonPropertyName("exitCode")]
    public int? ExitCode { get; init; }

    /// <summary>Whether the application is online.</summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }

    /// <summary>Whether the application was terminated for exceeding its RAM.</summary>
    [JsonPropertyName("ramKilled")]
    public bool? RamKilled { get; init; }

    /// <summary>The Git synchronization settings, when the application is linked to a repository.</summary>
    [JsonPropertyName("syncGit")]
    public SyncGit? SyncGit { get; init; }
}

/// <summary>
/// The Git synchronization settings of an application linked to a repository.
/// </summary>
public sealed record SyncGit
{
    /// <summary>Whether new commits are deployed automatically.</summary>
    [JsonPropertyName("autoDeploy")]
    public bool? AutoDeploy { get; init; }

    /// <summary>The identifier of the linked repository.</summary>
    [JsonPropertyName("repositoryID")]
    public long? RepositoryId { get; init; }

    /// <summary>The tracked branch.</summary>
    [JsonPropertyName("branchName")]
    public string? BranchName { get; init; }

    /// <summary>The last deployed commit.</summary>
    [JsonPropertyName("lastCommit")]
    public GitCommit? LastCommit { get; init; }
}

/// <summary>
/// A Git commit reference.
/// </summary>
public sealed record GitCommit
{
    /// <summary>The commit message.</summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>The commit SHA.</summary>
    [JsonPropertyName("sha")]
    public string? Sha { get; init; }
}

/// <summary>
/// The result of uploading a new application (<c>POST /upload</c>).
/// </summary>
public sealed record UploadResponse : DiscloudResponse
{
    /// <summary>The created application.</summary>
    [JsonPropertyName("app")]
    public UploadedApp? App { get; init; }

    /// <summary>The application logs, when it fails to start.</summary>
    [JsonPropertyName("logs")]
    public string? Logs { get; init; }
}

/// <summary>
/// The result of uploading a new version of an application (<c>PUT /app/{appID}/commit</c>).
/// </summary>
public sealed record AppCommitResponse : DiscloudResponse
{
    /// <summary>The application logs, when it fails to start.</summary>
    [JsonPropertyName("logs")]
    public string? Logs { get; init; }
}

/// <summary>
/// The fields to change on an application profile (<c>PUT /app/{appID}/profile</c>).
/// Leave a property <see langword="null"/> to keep its current value.
/// </summary>
public sealed record UpdateAppProfileRequest
{
    /// <summary>The new application name (up to 30 characters).</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>The URL of the new application avatar.</summary>
    [JsonPropertyName("avatarURL")]
    public string? AvatarUrl { get; init; }
}

internal sealed record AppResponse : DiscloudResponse
{
    [JsonPropertyName("apps")]
    [JsonConverter(typeof(SingleOrArrayJsonConverter<App>))]
    public IReadOnlyList<App>? Apps { get; init; }
}

internal sealed record AppRamUpdateRequest([property: JsonPropertyName("ramMB")] int RamMb);

internal sealed record AppAptRequest([property: JsonPropertyName("apt")] string Apt);
