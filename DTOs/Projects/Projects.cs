using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// A project: a group of applications that share a cluster and, optionally, a team.
/// </summary>
/// <remarks>Different routes return different subsets of these properties.</remarks>
public sealed record Project
{
    /// <summary>The unique identifier of the project.</summary>
    [JsonPropertyName("_id")]
    public string? Id { get; init; }

    /// <summary>The user ID of the project owner.</summary>
    [JsonPropertyName("ownerID")]
    public string? OwnerId { get; init; }

    /// <summary>The project name (up to 50 characters).</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>The project description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>The project icon.</summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; init; }

    /// <summary>The identifier of the linked team, or <see langword="null"/> when no team is linked.</summary>
    [JsonPropertyName("teamID")]
    public string? TeamId { get; init; }

    /// <summary>The cluster hosting the project applications.</summary>
    [JsonPropertyName("clusterName")]
    public string? ClusterName { get; init; }
}

/// <summary>
/// An application as listed inside a project.
/// </summary>
public sealed record ProjectApp
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("_id")]
    public string? Id { get; init; }

    /// <summary>The display name of the application.</summary>
    [JsonPropertyName("nameTAG")]
    public string? NameTag { get; init; }

    /// <summary>The cluster hosting the application.</summary>
    [JsonPropertyName("clusterName")]
    public string? ClusterName { get; init; }
}

/// <summary>
/// The details of a project, including its team and applications (<c>GET /projects/{projectID}</c>).
/// </summary>
public sealed record ProjectDetails
{
    /// <summary>The project.</summary>
    [JsonPropertyName("project")]
    public ProjectSummary? Project { get; init; }

    /// <summary>
    /// The linked team, or <see langword="null"/>. The API contract does not document the shape of this object,
    /// so it is exposed as raw JSON.
    /// </summary>
    [JsonPropertyName("team")]
    public JsonElement? Team { get; init; }

    /// <summary>The applications of the project.</summary>
    [JsonPropertyName("apps")]
    public IReadOnlyList<ProjectApp>? Apps { get; init; }
}

/// <summary>
/// A project summary, including the role of the current user.
/// </summary>
public sealed record ProjectSummary
{
    /// <summary>The unique identifier of the project.</summary>
    [JsonPropertyName("_id")]
    public string? Id { get; init; }

    /// <summary>The project name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>The role of the current user in the project (for example <c>owner</c>).</summary>
    [JsonPropertyName("memberRole")]
    public string? MemberRole { get; init; }
}

/// <summary>
/// The data used to create a project (<c>POST /projects</c>).
/// </summary>
public sealed record CreateProjectRequest
{
    /// <summary>The project name (up to 50 characters).</summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>An optional description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>An optional icon.</summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; init; }

    /// <summary>An optional team to link to the project.</summary>
    [JsonPropertyName("teamID")]
    public string? TeamId { get; init; }

    /// <summary>Optional identifiers of existing applications to add to the project.</summary>
    [JsonPropertyName("fromAppIDs")]
    public IReadOnlyList<string>? FromAppIds { get; init; }
}

/// <summary>
/// The fields to change on a project (<c>PATCH /projects/{projectID}</c>).
/// Leave a property <see langword="null"/> to keep its current value.
/// </summary>
public sealed record UpdateProjectRequest
{
    /// <summary>The new project name (up to 50 characters).</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>The new description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>The new icon.</summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; init; }
}

/// <summary>
/// The link between an application and a project, created by <c>PUT /projects/{projectID}/apps/{appID}</c>.
/// </summary>
public sealed record ProjectAppLink
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("_id")]
    public string? AppId { get; init; }

    /// <summary>The identifier of the project.</summary>
    [JsonPropertyName("projectID")]
    public string? ProjectId { get; init; }

    /// <summary>The cluster hosting the application.</summary>
    [JsonPropertyName("clusterName")]
    public string? ClusterName { get; init; }
}

/// <summary>
/// An asynchronous migration of an application to the cluster of a project.
/// </summary>
public sealed record ProjectMigration
{
    /// <summary>The unique identifier of the application being migrated.</summary>
    [JsonPropertyName("appID")]
    public string? AppId { get; init; }

    /// <summary>The identifier of the project.</summary>
    [JsonPropertyName("projectID")]
    public string? ProjectId { get; init; }

    /// <summary>The source cluster.</summary>
    [JsonPropertyName("from")]
    public string? From { get; init; }

    /// <summary>The destination cluster.</summary>
    [JsonPropertyName("to")]
    public string? To { get; init; }

    /// <summary>The estimated duration of the migration, in seconds.</summary>
    [JsonPropertyName("estimatedSec")]
    public int? EstimatedSeconds { get; init; }
}

/// <summary>
/// The outcome of adding an application to a project.
/// </summary>
/// <remarks>
/// When the application already runs on the project cluster it is linked immediately and <see cref="Link"/> is set.
/// When a migration was requested and is required, the API answers with HTTP 202, the migration runs in the
/// background and <see cref="Migration"/> is set instead.
/// </remarks>
public sealed record AddAppToProjectResult
{
    /// <summary>The message returned by the API.</summary>
    public string? Message { get; init; }

    /// <summary>The created link, when the application was added immediately.</summary>
    public ProjectAppLink? Link { get; init; }

    /// <summary>The started migration, when the application must move to the project cluster first.</summary>
    public ProjectMigration? Migration { get; init; }

    /// <summary>Gets a value indicating whether an asynchronous migration was started.</summary>
    public bool IsMigrationStarted => Migration is not null;
}

internal sealed record ProjectResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public Project? Data { get; init; }
}

internal sealed record ProjectListResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public IReadOnlyList<Project>? Data { get; init; }
}

internal sealed record ProjectDetailsResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public ProjectDetails? Data { get; init; }
}

internal sealed record ProjectAppListResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public IReadOnlyList<ProjectApp>? Data { get; init; }
}

internal sealed record ProjectAppLinkResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public ProjectAppLink? Data { get; init; }
}

internal sealed record ProjectMigrationResponse : DiscloudResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("data")]
    public ProjectMigration? Data { get; init; }
}

internal sealed record SetProjectTeamRequest
{
    // Must be written even when null: a null teamID unlinks the team.
    [JsonPropertyName("teamID")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? TeamId { get; init; }
}

internal sealed record AddAppToProjectRequest([property: JsonPropertyName("migrate")] bool Migrate);
