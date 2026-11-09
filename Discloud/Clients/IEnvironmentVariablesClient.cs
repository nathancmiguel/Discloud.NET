namespace Discloud.Clients;

/// <summary>
/// Manages environment variables at the account, project and application scopes (<c>/env</c>). Available through
/// <see cref="IDiscloudClient.EnvironmentVariables"/>.
/// </summary>
/// <remarks>
/// Values are returned decrypted: treat them as secrets and avoid logging them. Every <c>Set…</c> method replaces the
/// whole set of variables of its scope; read, modify and write back to change a single variable.
/// </remarks>
/// <example>
/// <code>
/// var vars = new Dictionary&lt;string, string&gt;(await client.EnvironmentVariables.GetForAppAsync(appId))
/// {
///     ["NODE_ENV"] = "production",
/// };
/// await client.EnvironmentVariables.SetForAppAsync(appId, vars);
/// </code>
/// </example>
public interface IEnvironmentVariablesClient
{
    /// <summary>Gets the account-scope variables (<c>GET /env/account</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The variables, keyed by name.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyDictionary<string, string>> GetForAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>Replaces all account-scope variables (<c>PUT /env/account</c>).</summary>
    /// <param name="variables">The complete new set of variables.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The saved variables.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="variables"/> is <see langword="null"/>.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request, for example for an invalid variable name.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyDictionary<string, string>> SetForAccountAsync(IReadOnlyDictionary<string, string> variables, CancellationToken cancellationToken = default);

    /// <summary>Removes all account-scope variables (<c>DELETE /env/account</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The number of removed variables.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<int> DeleteForAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the variables of a project (<c>GET /env/project/{projectID}</c>). Requires project management permission.</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The variables, keyed by name.</returns>
    /// <exception cref="ArgumentException"><paramref name="projectId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyDictionary<string, string>> GetForProjectAsync(string projectId, CancellationToken cancellationToken = default);

    /// <summary>Replaces all variables of a project (<c>PUT /env/project/{projectID}</c>).</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="variables">The complete new set of variables.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The saved variables.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="variables"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="projectId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyDictionary<string, string>> SetForProjectAsync(string projectId, IReadOnlyDictionary<string, string> variables, CancellationToken cancellationToken = default);

    /// <summary>Removes all variables of a project (<c>DELETE /env/project/{projectID}</c>).</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The number of removed variables.</returns>
    /// <exception cref="ArgumentException"><paramref name="projectId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<int> DeleteForProjectAsync(string projectId, CancellationToken cancellationToken = default);

    /// <summary>Gets the variables of an application (<c>GET /env/app/{appID}</c>). Requires the <c>config_app</c> permission.</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The variables, keyed by name.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyDictionary<string, string>> GetForAppAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Replaces all variables of an application (<c>PUT /env/app/{appID}</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="variables">The complete new set of variables.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The saved variables.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="variables"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyDictionary<string, string>> SetForAppAsync(string appId, IReadOnlyDictionary<string, string> variables, CancellationToken cancellationToken = default);

    /// <summary>Removes all variables of an application (<c>DELETE /env/app/{appID}</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The number of removed variables.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<int> DeleteForAppAsync(string appId, CancellationToken cancellationToken = default);
}
