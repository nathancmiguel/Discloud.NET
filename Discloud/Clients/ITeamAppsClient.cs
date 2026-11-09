using Discloud.DTOs;

namespace Discloud.Clients;

/// <summary>
/// Operations on applications owned by other users that you moderate (<c>/team</c>). Available through
/// <see cref="IDiscloudClient.TeamApps"/>.
/// </summary>
/// <remarks>
/// Each operation requires the matching <see cref="AppPermission"/> granted by the application owner.
/// Methods ending in <c>All</c> act on every application you moderate.
/// </remarks>
public interface ITeamAppsClient
{
    /// <summary>Lists the applications you moderate, with your permissions on each (<c>GET /team</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The applications you moderate.</returns>
    /// <exception cref="DiscloudForbiddenException">You do not moderate any application.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<TeamApp>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the resource usage of an application you moderate (<c>GET /team/{appID}/status</c>). Requires <see cref="AppPermission.StatusApp"/>.</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The application status.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppStatus> GetStatusAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Gets the logs of an application you moderate (<c>GET /team/{appID}/logs</c>). Requires <see cref="AppPermission.LogsApp"/>.</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The application logs.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppLogs> GetLogsAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Gets the logs of every application you moderate (<c>GET /team/all/logs</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The logs of each application.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<AppLogs>> GetAllLogsAsync(CancellationToken cancellationToken = default);

    /// <summary>Generates a backup of an application you moderate (<c>GET /team/{appID}/backup</c>). Requires <see cref="AppPermission.BackupApp"/>.</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The backup.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppBackup> GetBackupAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Generates a backup of every application you moderate (<c>GET /team/all/backup</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The backup of each application.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<AppBackup>> GetAllBackupsAsync(CancellationToken cancellationToken = default);

    /// <summary>Starts an application you moderate (<c>PUT /team/{appID}/start</c>). Requires <see cref="AppPermission.StartApp"/>.</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppStartResponse> StartAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Starts every application you moderate (<c>PUT /team/all/start</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome per application.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppStartResponse> StartAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Restarts an application you moderate (<c>PUT /team/{appID}/restart</c>). Requires <see cref="AppPermission.RestartApp"/>.</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppRestartResponse> RestartAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Restarts every application you moderate (<c>PUT /team/all/restart</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome per application.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppRestartResponse> RestartAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Stops an application you moderate (<c>PUT /team/{appID}/stop</c>). Requires <see cref="AppPermission.StopApp"/>.</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppStopResponse> StopAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Stops every application you moderate (<c>PUT /team/all/stop</c>).</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome per application.</returns>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppStopResponse> StopAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Changes the RAM of an application you moderate (<c>PUT /team/{appID}/ram</c>). Requires <see cref="AppPermission.EditRam"/>.</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="ramMb">The RAM in megabytes.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="ramMb"/> is not positive.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<DiscloudResponse> SetRamAsync(string appId, int ramMb, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces the code of an application you moderate with a new <c>.zip</c> package
    /// (<c>PUT /team/{appID}/commit</c>). Requires <see cref="AppPermission.CommitApp"/>.
    /// </summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="zipFile">The <c>.zip</c> content. It is read from its current position and is not disposed.</param>
    /// <param name="fileName">The file name sent to the API, for example <c>app.zip</c>.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome, and the application logs when it fails to start.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="zipFile"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="appId"/> or <paramref name="fileName"/> is empty, or <paramref name="zipFile"/> is not readable.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<AppCommitResponse> CommitAsync(string appId, Stream zipFile, string fileName, CancellationToken cancellationToken = default);
}
