using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// A moderator of one of your applications (<c>/app/{appID}/team</c>).
/// </summary>
public sealed record AppModerator
{
    /// <summary>The identifier of the application. Only sent when adding or editing a moderator.</summary>
    [JsonPropertyName("appID")]
    public string? AppId { get; init; }

    /// <summary>The user ID of the moderator.</summary>
    [JsonPropertyName("modID")]
    public string? ModeratorId { get; init; }

    /// <summary>The permissions granted to the moderator.</summary>
    [JsonPropertyName("perms")]
    public IReadOnlyList<AppPermission>? Permissions { get; init; }

    /// <summary>The URL of the moderator's avatar, if known.</summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; init; }

    /// <summary>The moderator's username, if known.</summary>
    [JsonPropertyName("username")]
    public string? Username { get; init; }
}

/// <summary>
/// Another user's application that you moderate (<c>GET /team</c>).
/// </summary>
public sealed record TeamApp
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>The name of the application.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>Whether the application is a bot or a site.</summary>
    [JsonPropertyName("type")]
    public AppType? Type { get; init; }

    /// <summary>Whether the application is online.</summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }

    /// <summary>The RAM allocated to the application, in megabytes.</summary>
    [JsonPropertyName("ram")]
    public int? Ram { get; init; }

    /// <summary>Whether the application was terminated for exceeding its RAM.</summary>
    [JsonPropertyName("ramKilled")]
    public bool? RamKilled { get; init; }

    /// <summary>The exit code of the application process, if it stopped.</summary>
    [JsonPropertyName("exitCode")]
    public int? ExitCode { get; init; }

    /// <summary>The programming language of the application.</summary>
    [JsonPropertyName("lang")]
    public string? Language { get; init; }

    /// <summary>The owner of the application.</summary>
    [JsonPropertyName("owner")]
    public TeamAppOwner? Owner { get; init; }

    /// <summary>Your permissions on the application.</summary>
    [JsonPropertyName("perms")]
    public IReadOnlyList<AppPermission>? Permissions { get; init; }
}

/// <summary>
/// The owner of an application you moderate.
/// </summary>
public sealed record TeamAppOwner
{
    /// <summary>The owner's name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>The URL of the owner's avatar.</summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; init; }
}

internal sealed record AppModeratorRequest(
    [property: JsonPropertyName("modID")] string ModeratorId,
    [property: JsonPropertyName("perms")] IReadOnlyList<AppPermission> Permissions);

internal sealed record AppModeratorListResponse : DiscloudResponse
{
    [JsonPropertyName("team")]
    public IReadOnlyList<AppModerator>? Team { get; init; }
}

internal sealed record AppModeratorResponse : DiscloudResponse
{
    [JsonPropertyName("app")]
    public AppModerator? App { get; init; }
}

internal sealed record TeamAppListResponse : DiscloudResponse
{
    [JsonPropertyName("apps")]
    public IReadOnlyList<TeamApp>? Apps { get; init; }
}
