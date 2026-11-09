using System.Net;
using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

internal sealed class ProjectsClient(DiscloudConnection connection) : IProjectsClient
{
    private static readonly DiscloudJsonContext Json = DiscloudJsonContext.Default;

    public async Task<Project> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var response = await connection.SendJsonAsync(
            HttpMethod.Post, "projects", request, Json.CreateProjectRequest, Json.ProjectResponse, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public async Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, "projects", Json.ProjectListResponse, cancellationToken).ConfigureAwait(false);
        return response.Data ?? [];
    }

    public async Task<ProjectDetails> GetAsync(string projectId, CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, ProjectPath(projectId), Json.ProjectDetailsResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public async Task<Project> UpdateAsync(string projectId, UpdateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var path = ProjectPath(projectId);
        ArgumentNullException.ThrowIfNull(request);
        var response = await connection.SendJsonAsync(
            HttpMethod.Patch, path, request, Json.UpdateProjectRequest, Json.ProjectResponse, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public Task<DiscloudResponse> DeleteAsync(string projectId, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Delete, ProjectPath(projectId), Json.DiscloudResponse, cancellationToken);

    public async Task<Project> SetTeamAsync(string projectId, string? teamId, CancellationToken cancellationToken = default)
    {
        var path = ProjectPath(projectId) + "/team";
        var response = await connection.SendJsonAsync(
            HttpMethod.Patch, path, new SetProjectTeamRequest { TeamId = teamId }, Json.SetProjectTeamRequest, Json.ProjectResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public async Task<IReadOnlyList<ProjectApp>> ListAvailableAppsAsync(string projectId, CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, ProjectPath(projectId) + "/apps", Json.ProjectAppListResponse, cancellationToken)
            .ConfigureAwait(false);
        return response.Data ?? [];
    }

    public Task<AddAppToProjectResult> AddAppAsync(string projectId, string appId, bool migrate = false, CancellationToken cancellationToken = default)
    {
        var path = AppPath(projectId, appId);
        var body = DiscloudConnection.CreateJsonContent(new AddAppToProjectRequest(migrate), Json.AddAppToProjectRequest);

        // HTTP 200 links the application immediately; HTTP 202 means a migration was started, with a different body.
        return connection.SendCoreAsync(
            HttpMethod.Put,
            path,
            body,
            async (response, ct) =>
            {
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    var migration = await DiscloudConnection.ReadResponseAsync(response, Json.ProjectMigrationResponse, HttpMethod.Put, path, ct)
                        .ConfigureAwait(false);
                    return new AddAppToProjectResult { Message = migration.Message, Migration = DiscloudConnection.Require(migration.Data, "data") };
                }

                var link = await DiscloudConnection.ReadResponseAsync(response, Json.ProjectAppLinkResponse, HttpMethod.Put, path, ct)
                    .ConfigureAwait(false);
                return new AddAppToProjectResult { Message = link.Message, Link = DiscloudConnection.Require(link.Data, "data") };
            },
            cancellationToken);
    }

    public Task<DiscloudResponse> RemoveAppAsync(string projectId, string appId, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Delete, AppPath(projectId, appId), Json.DiscloudResponse, cancellationToken);

    private static string ProjectPath(string projectId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        return $"projects/{ApiPath.Segment(projectId)}";
    }

    private static string AppPath(string projectId, string appId)
    {
        var path = ProjectPath(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(appId);
        return $"{path}/apps/{ApiPath.Segment(appId)}";
    }
}
