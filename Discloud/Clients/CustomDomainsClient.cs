using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

internal sealed class CustomDomainsClient(DiscloudConnection connection) : ICustomDomainsClient
{
    private static readonly DiscloudJsonContext Json = DiscloudJsonContext.Default;

    public async Task<IReadOnlyList<CustomDomain>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, "customdomain/all", Json.CustomDomainListResponse, cancellationToken)
            .ConfigureAwait(false);
        return response.Domains ?? [];
    }

    public Task<CustomDomain> GetAsync(string domain, CancellationToken cancellationToken = default)
        => GetDomainAsync(DomainPath(domain), cancellationToken);

    public async Task<CustomDomain> CreateAsync(string domain, string appId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);
        ArgumentException.ThrowIfNullOrWhiteSpace(appId);
        var response = await connection.SendJsonAsync(
            HttpMethod.Post,
            "customdomain/create",
            new CustomDomainCreateRequest(domain, appId),
            Json.CustomDomainCreateRequest,
            Json.CustomDomainResponse,
            cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.Domain, "domain");
    }

    public Task<CustomDomainMoveResponse> MoveAsync(string domain, string newAppId, CancellationToken cancellationToken = default)
    {
        var path = DomainPath(domain) + "/edit";
        ArgumentException.ThrowIfNullOrWhiteSpace(newAppId);
        return connection.SendJsonAsync(
            HttpMethod.Put, path, new CustomDomainMoveRequest(newAppId), Json.CustomDomainMoveRequest, Json.CustomDomainMoveResponse, cancellationToken);
    }

    public Task<CustomDomainRemoveResponse> RemoveAsync(string domain, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Delete, DomainPath(domain) + "/remove", Json.CustomDomainRemoveResponse, cancellationToken);

    public Task<CustomDomain> VerifyAsync(string domain, CancellationToken cancellationToken = default)
        => GetDomainAsync(DomainPath(domain) + "/verify", cancellationToken);

    private async Task<CustomDomain> GetDomainAsync(string path, CancellationToken cancellationToken)
    {
        var response = await connection.SendAsync(HttpMethod.Get, path, Json.CustomDomainResponse, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.Domain, "domain");
    }

    private static string DomainPath(string domain)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);
        return $"customdomain/{ApiPath.Segment(domain)}";
    }
}
