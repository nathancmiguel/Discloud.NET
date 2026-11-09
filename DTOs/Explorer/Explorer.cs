using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// An entry of a directory listing in the application file explorer.
/// </summary>
public sealed record ExplorerItem
{
    /// <summary>The file or directory name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>Whether the entry is a file or a directory.</summary>
    [JsonPropertyName("type")]
    public ExplorerItemType? Type { get; init; }

    /// <summary>The file size in bytes. Only sent for files.</summary>
    [JsonPropertyName("size")]
    public long? Size { get; init; }
}

/// <summary>
/// The contents of a directory (<c>GET /app/{appID}/explorer</c>).
/// </summary>
public sealed record ExplorerListResponse : DiscloudResponse
{
    /// <summary>The listed directory.</summary>
    [JsonPropertyName("path")]
    public string? Path { get; init; }

    /// <summary>The files and directories inside <see cref="Path"/>.</summary>
    [JsonPropertyName("dir")]
    public IReadOnlyList<ExplorerItem>? Items { get; init; }
}

/// <summary>
/// The contents of a file (<c>GET /app/{appID}/explorer/open</c>).
/// </summary>
public sealed record ExplorerOpenResponse : DiscloudResponse
{
    /// <summary>The full path of the file.</summary>
    [JsonPropertyName("path")]
    public string? Path { get; init; }

    /// <summary>The file name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>The file content.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; init; }
}

/// <summary>
/// The result of creating or editing an explorer entry.
/// </summary>
public sealed record ExplorerOperationResponse : DiscloudResponse
{
    /// <summary>The path affected by the operation.</summary>
    [JsonPropertyName("path")]
    public string? Path { get; init; }
}

internal sealed record ExplorerCreateRequest(
    [property: JsonPropertyName("cPath")] string Path,
    [property: JsonPropertyName("typeFile")] ExplorerItemType Type);

internal sealed record ExplorerEditRequest(
    [property: JsonPropertyName("cPath")] string Path,
    [property: JsonPropertyName("fileContent")] string Content);
