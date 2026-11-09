using Discloud.DTOs;

namespace Discloud.Clients;

/// <summary>
/// Manages versioned backups (snapshots) of applications (<c>/snapshot</c>). Available through
/// <see cref="IDiscloudClient.Snapshots"/>.
/// </summary>
/// <remarks>
/// Failures carry a machine-readable <see cref="DiscloudApiException.ErrorCode"/> such as
/// <c>SNAPSHOT:APP_BUSY</c> or <c>SNAPSHOT:PLAN_REQUIRED</c>. Creating snapshots requires the Ruby plan or higher.
/// </remarks>
public interface ISnapshotsClient
{
    /// <summary>Lists the snapshots of the account, optionally filtered by application (<c>GET /snapshot</c>).</summary>
    /// <param name="query">Paging and filters, or <see langword="null"/> for the API defaults.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>A page of snapshots, or the versions of one application when <see cref="SnapshotListQuery.AppId"/> is set.</returns>
    /// <exception cref="DiscloudNotFoundException">No snapshots exist for the requested application.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request, for example for an invalid page or limit.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<SnapshotListResponse> ListAsync(SnapshotListQuery? query = null, CancellationToken cancellationToken = default);

    /// <summary>Lists the stored versions of an application (<c>GET /snapshot/{appID}</c>).</summary>
    /// <param name="appId">The application identifier (letters, digits and hyphens, up to 40 characters).</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The stored versions.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">No snapshots exist for the application.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<IReadOnlyList<SnapshotVersion>> ListVersionsAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Creates a new snapshot of an application (<c>POST /snapshot/{appID}</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The created snapshot and the list of all stored versions.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudForbiddenException">You do not own the application, or the plan does not allow snapshots.</exception>
    /// <exception cref="DiscloudConflictException">The application is busy processing another task.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<CreatedSnapshot> CreateAsync(string appId, CancellationToken cancellationToken = default);

    /// <summary>Generates a temporary download URL for a snapshot version (<c>GET /snapshot/{appID}/versions/{version}</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="version">The version, formatted <c>YYYYMMDD-HHMMSS</c>.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The download URL and its expiration.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> or <paramref name="version"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The version does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<SnapshotDownload> GetDownloadUrlAsync(string appId, string version, CancellationToken cancellationToken = default);

    /// <summary>Permanently deletes a snapshot version (<c>DELETE /snapshot/{appID}/versions/{version}</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="version">The version, formatted <c>YYYYMMDD-HHMMSS</c>.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> or <paramref name="version"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The application or version does not exist.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<SnapshotDeleteResponse> DeleteAsync(string appId, string version, CancellationToken cancellationToken = default);
}
