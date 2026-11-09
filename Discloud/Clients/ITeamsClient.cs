using Discloud.DTOs;

namespace Discloud.Clients;

/// <summary>
/// Manages teams and their members (<c>/teams</c>). Available through <see cref="IDiscloudClient.Teams"/>.
/// </summary>
/// <remarks>
/// Teams grant shared access to projects. Not to be confused with <see cref="IDiscloudClient.TeamApps"/>, which
/// covers applications whose owners made you a moderator.
/// </remarks>
public interface ITeamsClient
{
    /// <summary>Creates a team (<c>POST /teams</c>). Requires a compatible plan.</summary>
    /// <param name="request">The team data.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The created team.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="DiscloudForbiddenException">The plan does not allow teams or the limit was reached.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<Team> CreateAsync(CreateTeamRequest request, CancellationToken cancellationToken = default);

    /// <summary>Lists the teams you own or belong to (<c>GET /teams</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The teams.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<Team>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Lists the applications you can access through your teams (<c>GET /teams/apps</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The accessible applications.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<TeamAccessibleApp>> ListAppsAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the permissions granted by a role (<c>GET /teams/roles/{role}/permissions</c>).</summary>
    /// <param name="role">The role: <see cref="TeamRole.Admin"/>, <see cref="TeamRole.Developer"/> or <see cref="TeamRole.Viewer"/>.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The role and its permissions.</returns>
    /// <exception cref="ArgumentException"><paramref name="role"/> is the default value.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request, for example for an invalid role.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<TeamRolePermissions> GetRolePermissionsAsync(TeamRole role, CancellationToken cancellationToken = default);

    /// <summary>Gets a team with its members and projects (<c>GET /teams/{teamID}</c>).</summary>
    /// <param name="teamId">The team identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The team details.</returns>
    /// <exception cref="ArgumentException"><paramref name="teamId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The team does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<TeamDetails> GetAsync(string teamId, CancellationToken cancellationToken = default);

    /// <summary>Changes the name and/or description of a team (<c>PATCH /teams/{teamID}</c>). Owner only.</summary>
    /// <param name="teamId">The team identifier.</param>
    /// <param name="request">The fields to change. <see langword="null"/> properties are left unchanged.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The updated team fields.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="teamId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<Team> UpdateAsync(string teamId, UpdateTeamRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a team (<c>DELETE /teams/{teamID}</c>). Owner only.</summary>
    /// <param name="teamId">The team identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="teamId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> DeleteAsync(string teamId, CancellationToken cancellationToken = default);

    /// <summary>Adds a member to a team (<c>POST /teams/{teamID}/members</c>).</summary>
    /// <param name="teamId">The team identifier.</param>
    /// <param name="userId">The user ID of the new member.</param>
    /// <param name="role">The role of the new member.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The added member.</returns>
    /// <exception cref="ArgumentException"><paramref name="teamId"/> or <paramref name="userId"/> is null, empty or whitespace, or <paramref name="role"/> is the default value.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<TeamMember> AddMemberAsync(string teamId, string userId, TeamRole role, CancellationToken cancellationToken = default);

    /// <summary>Lists the members of a team (<c>GET /teams/{teamID}/members</c>).</summary>
    /// <param name="teamId">The team identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The members.</returns>
    /// <exception cref="ArgumentException"><paramref name="teamId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<TeamMember>> ListMembersAsync(string teamId, CancellationToken cancellationToken = default);

    /// <summary>Changes the role of a team member (<c>PATCH /teams/{teamID}/members/{memberUserID}</c>).</summary>
    /// <param name="teamId">The team identifier.</param>
    /// <param name="userId">The user ID of the member.</param>
    /// <param name="role">The new role.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The updated member.</returns>
    /// <exception cref="ArgumentException"><paramref name="teamId"/> or <paramref name="userId"/> is null, empty or whitespace, or <paramref name="role"/> is the default value.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<TeamMember> UpdateMemberAsync(string teamId, string userId, TeamRole role, CancellationToken cancellationToken = default);

    /// <summary>Removes a member from a team (<c>DELETE /teams/{teamID}/members/{memberUserID}</c>).</summary>
    /// <param name="teamId">The team identifier.</param>
    /// <param name="userId">The user ID of the member.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="teamId"/> or <paramref name="userId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> RemoveMemberAsync(string teamId, string userId, CancellationToken cancellationToken = default);

    /// <summary>Leaves a team (<c>POST /teams/{teamID}/leave</c>). The owner cannot leave.</summary>
    /// <param name="teamId">The team identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="teamId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudForbiddenException">You own the team.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> LeaveAsync(string teamId, CancellationToken cancellationToken = default);
}
