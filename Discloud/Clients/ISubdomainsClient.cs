using Discloud.DTOs;

namespace Discloud.Clients;

/// <summary>
/// Manages <c>*.discloud.app</c> subdomains (<c>/subdomain</c>). Available through
/// <see cref="IDiscloudClient.Subdomains"/>.
/// </summary>
/// <remarks>Subdomains require the Platinum plan or higher. Each user can own up to 100 subdomains.</remarks>
public interface ISubdomainsClient
{
    /// <summary>Lists the subdomains of the account (<c>GET /subdomain/all</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The subdomains.</returns>
    /// <exception cref="DiscloudForbiddenException">The plan does not include subdomains.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<Subdomain>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a subdomain (<c>GET /subdomain/{subdomainName}</c>).</summary>
    /// <param name="name">The subdomain name, without the <c>.discloud.app</c> suffix.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The subdomain.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The subdomain does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<Subdomain> GetAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>Registers a new subdomain (<c>POST /subdomain/{subdomainName}</c>).</summary>
    /// <param name="name">The subdomain name: 2 to 20 characters among lowercase letters, digits and hyphens.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The created subdomain.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudConflictException">The subdomain is already registered.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request, for example for an invalid name or a reached limit.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<Subdomain> CreateAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>Deletes a subdomain that is not in use (<c>DELETE /subdomain/{subdomainName}</c>).</summary>
    /// <param name="name">The subdomain name.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudConflictException">The subdomain is in use by an application.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> DeleteAsync(string name, CancellationToken cancellationToken = default);
}
