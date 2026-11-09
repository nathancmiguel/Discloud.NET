using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

internal sealed class TeamAppsClient(DiscloudConnection connection) : AppRoutes(connection, "team"), ITeamAppsClient
{
    public async Task<IReadOnlyList<TeamApp>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await Connection.SendAsync(HttpMethod.Get, "team", Json.TeamAppListResponse, cancellationToken).ConfigureAwait(false);
        return response.Apps ?? [];
    }
}
