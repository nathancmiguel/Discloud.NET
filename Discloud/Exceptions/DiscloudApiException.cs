using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Discloud;

/// <summary>
/// Thrown when the Discloud API answers a request with an error.
/// </summary>
/// <remarks>
/// <para>
/// This covers unsuccessful HTTP status codes and also successful status codes whose body reports
/// <c>"status": "error"</c>, which some routes use (for example when starting an application that is already online).
/// In the latter case <see cref="StatusCode"/> is the successful status code returned by the server.
/// </para>
/// <para>
/// Specific status codes are raised as derived types: <see cref="DiscloudAuthenticationException"/> (401),
/// <see cref="DiscloudForbiddenException"/> (403), <see cref="DiscloudNotFoundException"/> (404),
/// <see cref="DiscloudConflictException"/> (409) and <see cref="DiscloudRateLimitException"/> (429).
/// </para>
/// </remarks>
/// <example>
/// <code>
/// try
/// {
///     await client.Apps.StartAsync("1234567890");
/// }
/// catch (DiscloudNotFoundException)
/// {
///     Console.WriteLine("The application does not exist.");
/// }
/// catch (DiscloudApiException ex)
/// {
///     Console.WriteLine($"{(int)ex.StatusCode} {ex.ErrorCode}: {ex.ApiMessage}");
/// }
/// </code>
/// </example>
public class DiscloudApiException : DiscloudException
{
    /// <summary>Initializes a new instance of the <see cref="DiscloudApiException"/> class.</summary>
    /// <param name="statusCode">The HTTP status code returned by the API.</param>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this one, if any.</param>
    public DiscloudApiException(HttpStatusCode statusCode, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }

    /// <summary>Gets the HTTP status code returned by the API.</summary>
    public HttpStatusCode StatusCode { get; }

    private string? _errorCode;
    private string? _apiMessage;
    private JsonElement? _errorData;

    /// <summary>
    /// Gets the machine-readable error code sent by the API, if any (for example <c>SNAPSHOT:APP_BUSY</c>,
    /// <c>CLUSTER_MISMATCH</c>, or a Discloud status code such as <c>93</c>).
    /// </summary>
    public string? ErrorCode { get => _errorCode; init => _errorCode = value; }

    /// <summary>Gets the human-readable message sent by the API, if any.</summary>
    public string? ApiMessage { get => _apiMessage; init => _apiMessage = value; }

    /// <summary>
    /// Gets additional structured details sent by the API, if any (for example the clusters involved in a
    /// <c>CLUSTER_MISMATCH</c>). The API contract does not define a schema for this object.
    /// </summary>
    public JsonElement? ErrorData { get => _errorData; init => _errorData = value; }

    internal static DiscloudApiException Create(
        HttpStatusCode statusCode,
        string? apiMessage,
        string? errorCode,
        JsonElement? errorData,
        TimeSpan? retryAfter,
        string method,
        string path)
    {
        var message = BuildMessage(statusCode, apiMessage, errorCode, method, path);
        DiscloudApiException exception = statusCode switch
        {
            HttpStatusCode.Unauthorized => new DiscloudAuthenticationException(message),
            HttpStatusCode.Forbidden => new DiscloudForbiddenException(message),
            HttpStatusCode.NotFound => new DiscloudNotFoundException(message),
            HttpStatusCode.Conflict => new DiscloudConflictException(message),
            HttpStatusCode.TooManyRequests => new DiscloudRateLimitException(message) { RetryAfter = retryAfter },
            _ => new DiscloudApiException(statusCode, message),
        };

        exception._apiMessage = apiMessage;
        exception._errorCode = errorCode;
        exception._errorData = errorData;
        exception.SetRequest(method, path);
        return exception;
    }

    private static string BuildMessage(HttpStatusCode statusCode, string? apiMessage, string? errorCode, string method, string path)
    {
        var builder = new StringBuilder("Discloud API request ")
            .Append(method).Append(' ').Append(path)
            .Append(" failed with HTTP ")
            .Append(((int)statusCode).ToString(CultureInfo.InvariantCulture))
            .Append(" (").Append(statusCode).Append(')');

        if (!string.IsNullOrWhiteSpace(errorCode))
        {
            builder.Append(" [").Append(errorCode).Append(']');
        }

        builder.Append(string.IsNullOrWhiteSpace(apiMessage) ? "." : ": " + apiMessage);
        return builder.ToString();
    }
}

/// <summary>
/// Thrown when the API rejects the token (HTTP 401). Check that the configured token is valid.
/// </summary>
public sealed class DiscloudAuthenticationException : DiscloudApiException
{
    /// <summary>Initializes a new instance of the <see cref="DiscloudAuthenticationException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public DiscloudAuthenticationException(string message)
        : base(HttpStatusCode.Unauthorized, message)
    {
    }
}

/// <summary>
/// Thrown when the account lacks the permission or the plan required by the operation (HTTP 403).
/// </summary>
public sealed class DiscloudForbiddenException : DiscloudApiException
{
    /// <summary>Initializes a new instance of the <see cref="DiscloudForbiddenException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public DiscloudForbiddenException(string message)
        : base(HttpStatusCode.Forbidden, message)
    {
    }
}

/// <summary>
/// Thrown when the requested resource does not exist or is not visible to the account (HTTP 404).
/// </summary>
public sealed class DiscloudNotFoundException : DiscloudApiException
{
    /// <summary>Initializes a new instance of the <see cref="DiscloudNotFoundException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public DiscloudNotFoundException(string message)
        : base(HttpStatusCode.NotFound, message)
    {
    }
}

/// <summary>
/// Thrown when the request conflicts with the current state of a resource (HTTP 409), for example when a name is
/// already taken or the resource is busy.
/// </summary>
public sealed class DiscloudConflictException : DiscloudApiException
{
    /// <summary>Initializes a new instance of the <see cref="DiscloudConflictException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public DiscloudConflictException(string message)
        : base(HttpStatusCode.Conflict, message)
    {
    }
}

/// <summary>
/// Thrown when too many requests were sent, or another operation is already running on the resource (HTTP 429).
/// </summary>
public sealed class DiscloudRateLimitException : DiscloudApiException
{
    /// <summary>Initializes a new instance of the <see cref="DiscloudRateLimitException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public DiscloudRateLimitException(string message)
        : base(HttpStatusCode.TooManyRequests, message)
    {
    }

    /// <summary>
    /// Gets how long to wait before retrying, from the <c>Retry-After</c> response header, when the API sends it.
    /// </summary>
    public TimeSpan? RetryAfter { get; init; }
}
