using Discloud.DTOs;

namespace Discloud.Clients;

/// <summary>
/// Manages the moderators of the applications you own (<c>/app/{appID}/team</c>). Available through
/// <see cref="IDiscloudClient.AppModerators"/>.
/// </summary>
public interface IAppModeratorsClient
{
    /// <summary>Lists the moderators of an application and their permissions (<c>GET /app/{appID}/team</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The moderators.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<AppModerator>> ListAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Adds a moderator to an application (<c>POST /app/{appID}/team</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="moderatorId">The user ID of the new moderator.</param>
    /// <param name="permissions">The permissions to grant.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The added moderator.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="permissions"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="appId"/> or <paramref name="moderatorId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    /// <example>
    /// <code>
    /// await client.AppModerators.AddAsync(appId, "197651928518230016",
    ///     [AppPermission.StartApp, AppPermission.StopApp, AppPermission.LogsApp]);
    /// </code>
    /// </example>
    Task<AppModerator> AddAsync(string appId, string moderatorId, IEnumerable<AppPermission> permissions, CancellationToken cancellationToken = default);

    /// <summary>Replaces the permissions of a moderator (<c>PUT /app/{appID}/team</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="moderatorId">The user ID of the moderator.</param>
    /// <param name="permissions">The complete new set of permissions.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The updated moderator.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="permissions"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="appId"/> or <paramref name="moderatorId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppModerator> UpdateAsync(string appId, string moderatorId, IEnumerable<AppPermission> permissions, CancellationToken cancellationToken = default);

    /// <summary>Removes a moderator from an application (<c>DELETE /app/{appID}/team/{modID}</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="moderatorId">The user ID of the moderator to remove.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> or <paramref name="moderatorId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> RemoveAsync(string appId, string moderatorId, CancellationToken cancellationToken = default);
}
