using Discloud.DTOs;

namespace Discloud.Clients;

/// <summary>
/// Operations on the authenticated account. Available through <see cref="IDiscloudClient.User"/>.
/// </summary>
public interface IUserClient
{
    /// <summary>
    /// Gets the authenticated user: plan, RAM usage, applications and domains (<c>GET /user</c>).
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The authenticated user.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request (for example an invalid token).</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    /// <example>
    /// <code>
    /// var user = await client.User.GetAsync();
    /// Console.WriteLine($"{user.Username}: {user.RamUsedMb}/{user.TotalRamMb} MB on plan {user.Plan}");
    /// </code>
    /// </example>
    Task<User> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the language of the messages returned by the API for this account (<c>PUT /locale/{locale}</c>).
    /// </summary>
    /// <param name="locale">The new locale, for example <see cref="DiscloudLocale.EnglishUnitedStates"/>.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The locale now in effect.</returns>
    /// <exception cref="DiscloudNotFoundException">The API does not support <paramref name="locale"/>.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudLocale> SetLocaleAsync(DiscloudLocale locale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the account data used by the Discloud VS Code extension, including a summary of every application and
    /// the identifiers of the applications the user moderates (<c>GET /vscode</c>).
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The account data.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<VscodeUser> GetVscodeDataAsync(CancellationToken cancellationToken = default);
}
