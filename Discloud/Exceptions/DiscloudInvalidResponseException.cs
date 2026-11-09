using System.Net;

namespace Discloud;

/// <summary>
/// Thrown when the API answers successfully but the body cannot be understood: it is empty, it is not valid JSON,
/// or it lacks the data the operation returns.
/// </summary>
/// <remarks>
/// When the body was malformed JSON, the original <see cref="System.Text.Json.JsonException"/> is available through
/// <see cref="Exception.InnerException"/>. The raw body is never included, because it may contain secrets such as
/// environment variables.
/// </remarks>
public sealed class DiscloudInvalidResponseException : DiscloudException
{
    /// <summary>Initializes a new instance of the <see cref="DiscloudInvalidResponseException"/> class.</summary>
    public DiscloudInvalidResponseException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DiscloudInvalidResponseException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public DiscloudInvalidResponseException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DiscloudInvalidResponseException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this one.</param>
    public DiscloudInvalidResponseException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>Gets the HTTP status code of the response, when known.</summary>
    public HttpStatusCode? StatusCode { get; init; }
}
