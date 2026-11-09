using Discloud.DTOs;

namespace Discloud.Clients;

/// <summary>
/// Manages projects, which group applications on one cluster and can be shared with a team (<c>/projects</c>).
/// Available through <see cref="IDiscloudClient.Projects"/>.
/// </summary>
public interface IProjectsClient
{
    /// <summary>Creates a project (<c>POST /projects</c>). Requires a compatible plan.</summary>
    /// <param name="request">The project data.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The created project.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="DiscloudForbiddenException">The plan does not allow projects or the limit was reached.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<Project> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);

    /// <summary>Lists the projects the user can access (<c>GET /projects</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The projects.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a project with its team and applications (<c>GET /projects/{projectID}</c>).</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The project details.</returns>
    /// <exception cref="ArgumentException"><paramref name="projectId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The project does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<ProjectDetails> GetAsync(string projectId, CancellationToken cancellationToken = default);

    /// <summary>Changes the name, description and/or icon of a project (<c>PATCH /projects/{projectID}</c>).</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="request">The fields to change. <see langword="null"/> properties are left unchanged.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The updated project fields.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="projectId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<Project> UpdateAsync(string projectId, UpdateProjectRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a project (<c>DELETE /projects/{projectID}</c>). Its applications are unlinked, not deleted.</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="projectId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> DeleteAsync(string projectId, CancellationToken cancellationToken = default);

    /// <summary>Links a team to a project, or unlinks it (<c>PATCH /projects/{projectID}/team</c>). Owner only.</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="teamId">The team to link, or <see langword="null"/> to unlink the current team.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The updated project fields.</returns>
    /// <exception cref="ArgumentException"><paramref name="projectId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<Project> SetTeamAsync(string projectId, string? teamId, CancellationToken cancellationToken = default);

    /// <summary>Lists the applications that can be added to a project (<c>GET /projects/{projectID}/apps</c>).</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The available applications.</returns>
    /// <exception cref="ArgumentException"><paramref name="projectId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<ProjectApp>> ListAvailableAppsAsync(string projectId, CancellationToken cancellationToken = default);

    /// <summary>Adds an application to a project (<c>PUT /projects/{projectID}/apps/{appID}</c>).</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="appId">The application identifier.</param>
    /// <param name="migrate">
    /// Whether to migrate the application to the project cluster when they differ. The migration runs in the background.
    /// </param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The created link, or the started migration when <paramref name="migrate"/> was needed.</returns>
    /// <exception cref="ArgumentException"><paramref name="projectId"/> or <paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudConflictException">
    /// The application runs on another cluster and <paramref name="migrate"/> is <see langword="false"/>
    /// (error code <c>CLUSTER_MISMATCH</c>, clusters in <see cref="DiscloudApiException.ErrorData"/>), or a migration is already running.
    /// </exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AddAppToProjectResult> AddAppAsync(string projectId, string appId, bool migrate = false, CancellationToken cancellationToken = default);

    /// <summary>Removes an application from a project (<c>DELETE /projects/{projectID}/apps/{appID}</c>). The application is not deleted.</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="projectId"/> or <paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> RemoveAppAsync(string projectId, string appId, CancellationToken cancellationToken = default);
}
