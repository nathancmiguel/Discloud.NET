using Discloud.DTOs;

namespace Discloud.Clients;

/// <summary>
/// Browses and edits the files of an application (<c>/app/{appID}/explorer</c>). Available through
/// <see cref="IDiscloudClient.Explorer"/>.
/// </summary>
/// <remarks>The file explorer requires a compatible plan; otherwise the API answers with HTTP 403.</remarks>
public interface IExplorerClient
{
    /// <summary>Lists the files and directories of a directory (<c>GET /app/{appID}/explorer</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="path">The directory to list, or <see langword="null"/> for the application working directory.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The directory contents.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudForbiddenException">No permission, or the plan does not include the file explorer.</exception>
    /// <exception cref="DiscloudNotFoundException">The application does not exist or the path is invalid.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<ExplorerListResponse> ListAsync(string appId, string? path = null, CancellationToken cancellationToken = default);

    /// <summary>Creates an empty file or a directory (<c>POST /app/{appID}/explorer</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="path">The full path of the new entry.</param>
    /// <param name="type">Whether to create a file or a directory.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The created path.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> or <paramref name="path"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<ExplorerOperationResponse> CreateAsync(string appId, string path, ExplorerItemType type, CancellationToken cancellationToken = default);

    /// <summary>Reads the content of a file (<c>GET /app/{appID}/explorer/open</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="path">The full path of the file.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The file content.</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> or <paramref name="path"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The file does not exist or the path is invalid.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<ExplorerOpenResponse> ReadFileAsync(string appId, string path, CancellationToken cancellationToken = default);

    /// <summary>Replaces the content of a file (<c>PUT /app/{appID}/explorer/edit</c>).</summary>
    /// <param name="appId">The application identifier.</param>
    /// <param name="path">The full path of the file.</param>
    /// <param name="content">The new file content.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The edited path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="appId"/> or <paramref name="path"/> is null, empty or whitespace.</exception>
    /// <exception cref="DiscloudNotFoundException">The file does not exist or the path is invalid.</exception>
    /// <exception cref="DiscloudApiException">The API rejected the request.</exception>
    /// <exception cref="DiscloudException">The API could not be reached or returned an invalid response.</exception>
    Task<ExplorerOperationResponse> WriteFileAsync(string appId, string path, string content, CancellationToken cancellationToken = default);
}
