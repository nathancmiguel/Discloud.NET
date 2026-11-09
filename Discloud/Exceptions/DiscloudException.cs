namespace Discloud;

/// <summary>
/// The base class of every exception thrown by the Discloud SDK.
/// </summary>
/// <remarks>
/// Catch this type to handle any SDK failure. Catch a derived type to distinguish between:
/// <list type="bullet">
/// <item><description><see cref="DiscloudApiException"/>: the API answered and rejected the request.</description></item>
/// <item><description><see cref="DiscloudNetworkException"/>: the API could not be reached or the request timed out.</description></item>
/// <item><description><see cref="DiscloudInvalidResponseException"/>: the API answered with a body the SDK could not understand.</description></item>
/// </list>
/// Cancellation requested through a <see cref="CancellationToken"/> is reported as
/// <see cref="OperationCanceledException"/>, never as a <see cref="DiscloudException"/>.
/// </remarks>
public class DiscloudException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="DiscloudException"/> class.</summary>
    public DiscloudException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DiscloudException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public DiscloudException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DiscloudException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this one.</param>
    public DiscloudException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    private string? _requestMethod;
    private string? _requestPath;

    /// <summary>Gets the HTTP method of the failed request, when known.</summary>
    public string? RequestMethod { get => _requestMethod; init => _requestMethod = value; }

    /// <summary>
    /// Gets the path of the failed request relative to the API base address (for example <c>app/123/status</c>),
    /// when known. It never contains the API token.
    /// </summary>
    public string? RequestPath { get => _requestPath; init => _requestPath = value; }

    internal void SetRequest(string method, string path)
    {
        _requestMethod = method;
        _requestPath = path;
    }
}
