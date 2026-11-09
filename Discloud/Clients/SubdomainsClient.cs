using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

internal sealed class SubdomainsClient(DiscloudConnection connection) : ISubdomainsClient
{
    private static readonly DiscloudJsonContext Json = DiscloudJsonContext.Default;

    public async Task<IReadOnlyList<Subdomain>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, "subdomain/all", Json.SubdomainListResponse, cancellationToken)
            .ConfigureAwait(false);
        return response.Subdomains ?? [];
    }

    public Task<Subdomain> GetAsync(string name, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Get, name, cancellationToken);

    public Task<Subdomain> CreateAsync(string name, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Post, name, cancellationToken);

    public Task<DiscloudResponse> DeleteAsync(string name, CancellationToken cancellationToken = default)
        => connection.SendAsync(HttpMethod.Delete, SubdomainPath(name), Json.DiscloudResponse, cancellationToken);

    private async Task<Subdomain> SendAsync(HttpMethod method, string name, CancellationToken cancellationToken)
    {
        var response = await connection.SendAsync(method, SubdomainPath(name), Json.SubdomainResponse, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.Subdomain, "subdomain");
    }

    private static string SubdomainPath(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return $"subdomain/{ApiPath.Segment(name)}";
    }
}
