using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// Filters and paging for <c>GET /snapshot</c>. Leave a property <see langword="null"/> to use the API default.
/// </summary>
public sealed record SnapshotListQuery
{
    /// <summary>The page number, starting at 1. The API default is 1.</summary>
    public int? Page { get; init; }

    /// <summary>The number of items per page, from 1 to 200. The API default is 50.</summary>
    public int? Limit { get; init; }

    /// <summary>Whether to return a summarized view of each snapshot. The API default is <see langword="false"/>.</summary>
    public bool? Summary { get; init; }

    /// <summary>
    /// Restricts the result to one application (letters, digits and hyphens, up to 40 characters).
    /// When set, the response contains <see cref="SnapshotListResponse.App"/> and
    /// <see cref="SnapshotListResponse.Versions"/> instead of a page of snapshots.
    /// </summary>
    public string? AppId { get; init; }
}

/// <summary>
/// The versioned backups (snapshots) of the user (<c>GET /snapshot</c>).
/// </summary>
/// <remarks>
/// Without <see cref="SnapshotListQuery.AppId"/>, the response is a page of snapshots across all applications
/// (<see cref="Page"/>, <see cref="Limit"/>, <see cref="Total"/> and <see cref="Backups"/>). With it, the response
/// lists the versions of that application (<see cref="App"/> and <see cref="Versions"/>).
/// </remarks>
public sealed record SnapshotListResponse : DiscloudResponse
{
    /// <summary>The current page.</summary>
    [JsonPropertyName("page")]
    public int? Page { get; init; }

    /// <summary>The page size.</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; init; }

    /// <summary>The total number of snapshots.</summary>
    [JsonPropertyName("total")]
    public int? Total { get; init; }

    /// <summary>
    /// The snapshot entries of the page. The API contract does not document the shape of these entries, so they
    /// are exposed as raw JSON.
    /// </summary>
    [JsonPropertyName("backups")]
    public IReadOnlyList<JsonElement>? Backups { get; init; }

    /// <summary>The application the versions belong to, when filtering by application.</summary>
    [JsonPropertyName("app")]
    public SnapshotAppReference? App { get; init; }

    /// <summary>The versions of the application, when filtering by application.</summary>
    [JsonPropertyName("versions")]
    public IReadOnlyList<SnapshotVersion>? Versions { get; init; }
}

/// <summary>
/// A reference to the application a snapshot belongs to.
/// </summary>
public sealed record SnapshotAppReference
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }
}

/// <summary>
/// A stored version of an application snapshot.
/// </summary>
public sealed record SnapshotVersion
{
    /// <summary>The version identifier, formatted <c>YYYYMMDD-HHMMSS</c>.</summary>
    [JsonPropertyName("version")]
    public string? Version { get; init; }

    /// <summary>The snapshot size in bytes.</summary>
    [JsonPropertyName("size")]
    public long? Size { get; init; }

    /// <summary>When the snapshot was created. Sent by <c>GET /snapshot?appID=…</c>.</summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>When the snapshot was created, as Unix time in milliseconds. Sent by <c>GET /snapshot/{appID}</c>.</summary>
    [JsonPropertyName("date")]
    public long? Date { get; init; }
}

/// <summary>
/// A temporary download link for a snapshot version.
/// </summary>
public sealed record SnapshotDownload
{
    /// <summary>The temporary download URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>When <see cref="Url"/> expires.</summary>
    [JsonPropertyName("expiresAt")]
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>The snapshot version.</summary>
    [JsonPropertyName("version")]
    public string? Version { get; init; }

    /// <summary>The snapshot size in bytes.</summary>
    [JsonPropertyName("size")]
    public long? Size { get; init; }
}

/// <summary>
/// The result of deleting a snapshot version (<c>DELETE /snapshot/{appID}/versions/{version}</c>).
/// </summary>
public sealed record SnapshotDeleteResponse : DiscloudResponse
{
    /// <summary>The application the snapshot belonged to.</summary>
    [JsonPropertyName("app")]
    public SnapshotAppReference? App { get; init; }

    /// <summary>The storage key of the deleted snapshot.</summary>
    [JsonPropertyName("deleted")]
    public string? Deleted { get; init; }
}

/// <summary>
/// A snapshot created by <c>POST /snapshot/{appID}</c>.
/// </summary>
public sealed record CreatedSnapshot
{
    /// <summary>The version identifier of the new snapshot, formatted <c>YYYYMMDD-HHMMSS</c>.</summary>
    [JsonPropertyName("version")]
    public string? Version { get; init; }

    /// <summary>The snapshot size, formatted by the API (for example <c>2.5 MB</c>).</summary>
    [JsonPropertyName("size")]
    public string? Size { get; init; }

    /// <summary>The download URL of the snapshot.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>All stored versions of the application, including the new one.</summary>
    [JsonPropertyName("allVersions")]
    public IReadOnlyList<CreatedSnapshotVersion>? AllVersions { get; init; }
}

/// <summary>
/// A stored snapshot version, as listed after creating a snapshot.
/// </summary>
public sealed record CreatedSnapshotVersion
{
    /// <summary>The version identifier, formatted <c>YYYYMMDD-HHMMSS</c>.</summary>
    [JsonPropertyName("version")]
    public string? Version { get; init; }

    /// <summary>The snapshot size, formatted by the API (for example <c>2.5 MB</c>).</summary>
    [JsonPropertyName("size")]
    public string? Size { get; init; }

    /// <summary>The creation date as sent by the API (an ISO 8601 timestamp).</summary>
    [JsonPropertyName("date")]
    public string? Date { get; init; }
}

internal sealed record SnapshotDownloadResponse : DiscloudResponse
{
    [JsonPropertyName("app")]
    public SnapshotAppReference? App { get; init; }

    [JsonPropertyName("download")]
    public SnapshotDownload? Download { get; init; }
}

internal sealed record AppSnapshotListResponse : DiscloudResponse
{
    [JsonPropertyName("app")]
    public SnapshotAppReference? App { get; init; }

    [JsonPropertyName("versions")]
    public IReadOnlyList<SnapshotVersion>? Versions { get; init; }
}

internal sealed record SnapshotCreateResponse : DiscloudResponse
{
    [JsonPropertyName("app")]
    public SnapshotAppReference? App { get; init; }

    [JsonPropertyName("snapshot")]
    public CreatedSnapshot? Snapshot { get; init; }
}
