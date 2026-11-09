using Discloud.DTOs;

namespace Discloud.Clients;

/// <summary>
/// Manages custom domains linked to applications (<c>/customdomain</c>). Available through
/// <see cref="IDiscloudClient.CustomDomains"/>.
/// </summary>
public interface ICustomDomainsClient
{
    /// <summary>Lists the custom domains of the account (<c>GET /customdomain/all</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The custom domains.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<CustomDomain>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a custom domain (<c>GET /customdomain/{domainName}</c>).</summary>
    /// <param name="domain">The domain name, for example <c>example.com</c>.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The custom domain.</returns>
    /// <exception cref="ArgumentException"><paramref name="domain"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The domain does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<CustomDomain> GetAsync(string domain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a custom domain and links it to an application (<c>POST /customdomain/create</c>). Create the
    /// records listed in <see cref="CustomDomain.Instructions"/> at your DNS provider afterwards.
    /// </summary>
    /// <param name="domain">The domain name.</param>
    /// <param name="appId">The application to link the domain to.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The registered domain, including its DNS instructions.</returns>
    /// <exception cref="ArgumentException"><paramref name="domain"/> or <paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudConflictException">The domain is already registered, or the application already has a custom domain.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<CustomDomain> CreateAsync(string domain, string appId, CancellationToken cancellationToken = default);

    /// <summary>Moves a custom domain to another application (<c>PUT /customdomain/{domainName}/edit</c>).</summary>
    /// <param name="domain">The domain name.</param>
    /// <param name="newAppId">The application that should receive the domain.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="domain"/> or <paramref name="newAppId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudConflictException">The target application already has another custom domain.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<CustomDomainMoveResponse> MoveAsync(string domain, string newAppId, CancellationToken cancellationToken = default);

    /// <summary>Removes a custom domain and unlinks it from its application (<c>DELETE /customdomain/{domainName}/remove</c>).</summary>
    /// <param name="domain">The domain name.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="domain"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<CustomDomainRemoveResponse> RemoveAsync(string domain, CancellationToken cancellationToken = default);

    /// <summary>Checks the DNS records of a custom domain in real time (<c>GET /customdomain/{domainName}/verify</c>).</summary>
    /// <param name="domain">The domain name.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The domain with <see cref="CustomDomain.Valid"/> and <see cref="CustomDomain.Message"/> set.</returns>
    /// <exception cref="ArgumentException"><paramref name="domain"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<CustomDomain> VerifyAsync(string domain, CancellationToken cancellationToken = default);
}
