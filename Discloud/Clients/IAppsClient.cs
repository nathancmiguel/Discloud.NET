using Discloud.DTOs;

namespace Discloud.Clients;

/// <summary>
/// Operations on the applications you own. Available through <see cref="IDiscloudClient.Apps"/>.
/// </summary>
/// <remarks>
/// Methods ending in <c>All</c> act on every application of the account at once (the API's special <c>all</c>
/// identifier). To manage applications owned by other users that you moderate, use <see cref="IDiscloudClient.TeamApps"/>.
/// </remarks>
public interface IAppsClient
{
    /// <summary>
    /// Uploads a new application packaged as a <c>.zip</c> file containing a <c>discloud.config</c>
    /// (<c>POST /upload</c>).
    /// </summary>
    /// <param name="zipFile">The <c>.zip</c> content. It is read from its current position and is not disposed.</param>
    /// <param name="fileName">The file name sent to the API, for example <c>app.zip</c>.</param>
    /// <param name="projectId">An optional project to link the application to.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The created application, and its logs when it fails to start.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="zipFile"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty, or <paramref name="zipFile"/> is not readable.</exception>
    /// <exception cref="DiscloudApiException">
    /// The API rejected the upload, for example because <c>discloud.config</c> is missing (error code <c>93</c>) or the
    /// plan limit was reached.
    /// </exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    /// <example>
    /// <code>
    /// await using var zip = File.OpenRead("bot.zip");
    /// var result = await client.Apps.UploadAsync(zip, "bot.zip");
    /// Console.WriteLine($"Uploaded {result.App?.Id}");
    /// </code>
    /// </example>
    Task<UploadResponse> UploadAsync(Stream zipFile, string fileName, string? projectId = null, CancellationToken cancellationToken = default);

    /// <summary>Gets an application (<c>GET /app/{appID}</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The application.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The application does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<App> GetAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Gets every application of the account (<c>GET /app/all</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The applications.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<App>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the resource usage of an application (<c>GET /app/{appID}/status</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The CPU, memory, storage and network usage of the application.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The application does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppStatus> GetStatusAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Gets the resource usage of every application (<c>GET /app/all/status</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The status of each application.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<AppStatus>> GetAllStatusesAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the recent terminal output of an application (<c>GET /app/{appID}/logs</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The application logs.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The application does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppLogs> GetLogsAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Gets the recent terminal output of every application (<c>GET /app/all/logs</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The logs of each application.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<AppLogs>> GetAllLogsAsync(CancellationToken cancellationToken = default);

    /// <summary>Generates a backup of an application and returns its download URL (<c>GET /app/{appID}/backup</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The backup.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The application does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppBackup> GetBackupAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Generates a backup of every application (<c>GET /app/all/backup</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The backup of each application.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<AppBackup>> GetAllBackupsAsync(CancellationToken cancellationToken = default);

    /// <summary>Starts an application (<c>PUT /app/{appID}/start</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome, including <see cref="AppStartResponse.AppStatus"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The application does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request, for example because the application is already online.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppStartResponse> StartAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Starts every application (<c>PUT /app/all/start</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome per application in <see cref="AppStartResponse.Apps"/>.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppStartResponse> StartAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Restarts an application (<c>PUT /app/{appID}/restart</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The application does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppRestartResponse> RestartAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Restarts every application (<c>PUT /app/all/restart</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome per application in <see cref="AppRestartResponse.Apps"/>.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppRestartResponse> RestartAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Stops an application (<c>PUT /app/{appID}/stop</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The application does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request, for example because the application is already stopped.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppStopResponse> StopAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Stops every application (<c>PUT /app/all/stop</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome per application in <see cref="AppStopResponse.Apps"/>.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppStopResponse> StopAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Changes the RAM allocated to an application (<c>PUT /app/{appID}/ram</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="ramMb">The RAM in megabytes. The minimum is 100 MB for bots and 512 MB for sites.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="ramMb"/> is not positive.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request, for example for insufficient RAM or plan.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> SetRamAsync(string appId, int ramMb, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces the code of an application with a new <c>.zip</c> package (<c>PUT /app/{appID}/commit</c>).
    /// </summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="zipFile">The <c>.zip</c> content. It is read from its current position and is not disposed.</param>
    /// <param name="fileName">The file name sent to the API, for example <c>app.zip</c>.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome, and the application logs when it fails to start.</returns>
    /// <remarks>
    /// The API documents that a successful commit may take long enough to time out without a response. Consider
    /// raising <see cref="DiscloudClientOptions.Timeout"/> for large applications.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="zipFile"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="appId"/> or <paramref name="fileName"/> is empty, or <paramref name="zipFile"/> is not readable.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppCommitResponse> CommitAsync(string appId, Stream zipFile, string fileName, CancellationToken cancellationToken = default);

    /// <summary>Changes the name and/or avatar of an application (<c>PUT /app/{appID}/profile</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="request">The fields to change. <see langword="null"/> properties are left unchanged.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> UpdateProfileAsync(string appId, UpdateAppProfileRequest request, CancellationToken cancellationToken = default);

    /// <summary>Installs APT packages on an application (<c>PUT /app/{appID}/apt</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="packages">The packages to install, for example <see cref="AptPackage.Ffmpeg"/>.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="packages"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is empty, or <paramref name="packages"/> is empty.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> InstallAptAsync(string appId, IEnumerable<AptPackage> packages, CancellationToken cancellationToken = default);

    /// <summary>Uninstalls APT packages from an application (<c>DELETE /app/{appID}/apt</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="packages">The packages to uninstall.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="packages"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is empty, or <paramref name="packages"/> is empty.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> UninstallAptAsync(string appId, IEnumerable<AptPackage> packages, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a command to the terminal of an application and returns its output (<c>PUT /app/{appID}/console</c>).
    /// </summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="command">The command to send, for example <c>ls</c>.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The command output.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> or <paramref name="command"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppConsoleResult> RunConsoleCommandAsync(string appId, string command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a command inside the application container (<c>PUT /app/{appID}/exec</c>). Only the owner can
    /// execute commands.
    /// </summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="command">The command to execute, for example <c>ls -la</c>.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The standard output, standard error and exit code of the command.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> or <paramref name="command"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The application does not exist or is offline.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<ExecResponse> ExecAsync(string appId, string command, CancellationToken cancellationToken = default);

    /// <summary>Permanently deletes an application (<c>DELETE /app/{appID}/delete</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The application does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppDeleteResponse> DeleteAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Permanently deletes every application of the account (<c>DELETE /app/all/delete</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome per application in <see cref="AppDeleteResponse.Apps"/>.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppDeleteResponse> DeleteAllAsync(CancellationToken cancellationToken = default);
}
