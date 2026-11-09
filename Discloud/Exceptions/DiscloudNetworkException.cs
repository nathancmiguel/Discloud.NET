namespace Discloud;

/// <summary>
/// Thrown when the Discloud API could not be reached, the connection failed while reading the response, or the
/// request exceeded <see cref="DiscloudClientOptions.Timeout"/>.
/// </summary>
/// <remarks>
/// The original <see cref="HttpRequestException"/>, <see cref="IOException"/> or <see cref="TimeoutException"/> is
/// available through <see cref="Exception.InnerException"/>. These failures are usually transient and safe to retry
/// for read operations.
/// </remarks>
public sealed class DiscloudNetworkException : DiscloudException
{
    /// <summary>Initializes a new instance of the <see cref="DiscloudNetworkException"/> class.</summary>
    public DiscloudNetworkException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DiscloudNetworkException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public DiscloudNetworkException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DiscloudNetworkException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The transport exception that caused this one.</param>
    public DiscloudNetworkException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>Gets a value indicating whether the request failed because it exceeded the configured timeout.</summary>
    public bool IsTimeout => InnerException is TimeoutException || InnerException?.InnerException is TimeoutException;
}
