using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// A team that groups users with shared access to projects.
/// </summary>
/// <remarks>Different routes return different subsets of these properties.</remarks>
public sealed record Team
{
    /// <summary>The unique identifier of the team.</summary>
    [JsonPropertyName("_id")]
    public string? Id { get; init; }

    /// <summary>The user ID of the team owner.</summary>
    [JsonPropertyName("ownerID")]
    public string? OwnerId { get; init; }

    /// <summary>The team name (up to 50 characters).</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>The team description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>Whether the current user owns the team.</summary>
    [JsonPropertyName("isOwner")]
    public bool? IsOwner { get; init; }
}

/// <summary>
/// A member of a team.
/// </summary>
public sealed record TeamMember
{
    /// <summary>The user ID of the member.</summary>
    [JsonPropertyName("userID")]
    public string? UserId { get; init; }

    /// <summary>The role of the member. The owner is reported as <see cref="TeamRole.Owner"/>.</summary>
    [JsonPropertyName("role")]
    public TeamRole? Role { get; init; }

    /// <summary>The member's username, if known.</summary>
    [JsonPropertyName("username")]
    public string? Username { get; init; }
}

/// <summary>
/// The details of a team, including its members and projects (<c>GET /teams/{teamID}</c>).
/// </summary>
public sealed record TeamDetails
{
    /// <summary>The team.</summary>
    [JsonPropertyName("team")]
    public Team? Team { get; init; }

    /// <summary>The members of the team.</summary>
    [JsonPropertyName("members")]
    public IReadOnlyList<TeamMember>? Members { get; init; }

    /// <summary>The projects linked to the team.</summary>
    [JsonPropertyName("projects")]
    public IReadOnlyList<TeamProject>? Projects { get; init; }
}

/// <summary>
/// A project linked to a team.
/// </summary>
public sealed record TeamProject
{
    /// <summary>The unique identifier of the project.</summary>
    [JsonPropertyName("_id")]
    public string? Id { get; init; }

    /// <summary>The project name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// An application the current user can access through a team (<c>GET /teams/apps</c>).
/// </summary>
public sealed record TeamAccessibleApp
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("_id")]
    public string? Id { get; init; }

    /// <summary>The display name of the application.</summary>
    [JsonPropertyName("nameTAG")]
    public string? NameTag { get; init; }

    /// <summary>The team granting access to the application.</summary>
    [JsonPropertyName("teamID")]
    public string? TeamId { get; init; }
}

/// <summary>
/// The permissions granted by a team role (<c>GET /teams/roles/{role}/permissions</c>).
/// </summary>
public sealed record TeamRolePermissions
{
    /// <summary>The role.</summary>
    [JsonPropertyName("role")]
    public TeamRole? Role { get; init; }

    /// <summary>The permissions granted by the role (for example <c>config_app</c> or <c>view_logs</c>).</summary>
    [JsonPropertyName("permissions")]
    public IReadOnlyList<string>? Permissions { get; init; }
}

/// <summary>
/// The data used to create a team (<c>POST /teams</c>).
/// </summary>
public sealed record CreateTeamRequest
{
    /// <summary>The team name (up to 50 characters).</summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>An optional description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

/// <summary>
/// The fields to change on a team (<c>PATCH /teams/{teamID}</c>).
/// Leave a property <see langword="null"/> to keep its current value.
/// </summary>
public sealed record UpdateTeamRequest
{
    /// <summary>The new team name (up to 50 characters).</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>The new description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

internal sealed record TeamResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public Team? Data { get; init; }
}

internal sealed record TeamListResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public IReadOnlyList<Team>? Data { get; init; }
}

internal sealed record TeamsAppListResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public IReadOnlyList<TeamAccessibleApp>? Data { get; init; }
}

internal sealed record TeamRolePermissionsResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public TeamRolePermissions? Data { get; init; }
}

internal sealed record TeamDetailsResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public TeamDetails? Data { get; init; }
}

internal sealed record TeamMemberResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public TeamMember? Data { get; init; }
}

internal sealed record TeamMemberListResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public IReadOnlyList<TeamMember>? Data { get; init; }
}

internal sealed record AddTeamMemberRequest(
    [property: JsonPropertyName("userID")] string UserId,
    [property: JsonPropertyName("role")] TeamRole Role);

internal sealed record UpdateTeamMemberRequest([property: JsonPropertyName("role")] TeamRole Role);
