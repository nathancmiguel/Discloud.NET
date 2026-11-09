using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

internal sealed class TeamsClient(DiscloudConnection connection) : ITeamsClient
{
    private static readonly DiscloudJsonContext Json = DiscloudJsonContext.Default;

    public async Task<Team> CreateAsync(CreateTeamRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var response = await connection.SendJsonAsync(HttpMethod.Post, "teams", request, Json.CreateTeamRequest, Json.TeamResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public async Task<IReadOnlyList<Team>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, "teams", Json.TeamListResponse, cancellationToken).ConfigureAwait(false);
        return response.Data ?? [];
    }

    public async Task<IReadOnlyList<TeamAccessibleApp>> ListAppsAsync(CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, "teams/apps", Json.TeamsAppListResponse, cancellationToken).ConfigureAwait(false);
        return response.Data ?? [];
    }

    public async Task<TeamRolePermissions> GetRolePermissionsAsync(TeamRole role, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role.Value, nameof(role));
        var path = $"teams/roles/{ApiPath.Segment(role.Value)}/permissions";
        var response = await connection.SendAsync(HttpMethod.Get, path, Json.TeamRolePermissionsResponse, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public async Task<TeamDetails> GetAsync(string teamId, CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, TeamPath(teamId), Json.TeamDetailsResponse, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public async Task<Team> UpdateAsync(string teamId, UpdateTeamRequest request, CancellationToken cancellationToken = default)
    {
        var path = TeamPath(teamId);
        ArgumentNullException.ThrowIfNull(request);
        var response = await connection.SendJsonAsync(HttpMethod.Patch, path, request, Json.UpdateTeamRequest, Json.TeamResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public Task<DiscloudResponse> DeleteAsync(string teamId, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Delete, TeamPath(teamId), Json.DiscloudResponse, cancellationToken);

    public async Task<TeamMember> AddMemberAsync(string teamId, string userId, TeamRole role, CancellationToken cancellationToken = default)
    {
        var path = TeamPath(teamId) + "/members";
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(role.Value, nameof(role));
        var response = await connection.SendJsonAsync(
            HttpMethod.Post, path, new AddTeamMemberRequest(userId, role), Json.AddTeamMemberRequest, Json.TeamMemberResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public async Task<IReadOnlyList<TeamMember>> ListMembersAsync(string teamId, CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, TeamPath(teamId) + "/members", Json.TeamMemberListResponse, cancellationToken)
            .ConfigureAwait(false);
        return response.Data ?? [];
    }

    public async Task<TeamMember> UpdateMemberAsync(string teamId, string userId, TeamRole role, CancellationToken cancellationToken = default)
    {
        var path = MemberPath(teamId, userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(role.Value, nameof(role));
        var response = await connection.SendJsonAsync(
            HttpMethod.Patch, path, new UpdateTeamMemberRequest(role), Json.UpdateTeamMemberRequest, Json.TeamMemberResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.Data, "data");
    }

    public Task<DiscloudResponse> RemoveMemberAsync(string teamId, string userId, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Delete, MemberPath(teamId, userId), Json.DiscloudResponse, cancellationToken);

    public Task<DiscloudResponse> LeaveAsync(string teamId, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Post, TeamPath(teamId) + "/leave", Json.DiscloudResponse, cancellationToken);

    private static string TeamPath(string teamId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);
        return $"teams/{ApiPath.Segment(teamId)}";
    }

    private static string MemberPath(string teamId, string userId)
    {
        var path = TeamPath(teamId);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return $"{path}/members/{ApiPath.Segment(userId)}";
    }
}
