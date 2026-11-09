using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Discloud.DTOs;

namespace Discloud.Http;

/// <summary>
/// Sends requests to the Discloud API and turns responses into DTOs or exceptions.
/// </summary>
/// <remarks>
/// Every header is set on the individual <see cref="HttpRequestMessage"/>: the shared <see cref="HttpClient"/> is
/// never mutated, so concurrent requests cannot observe each other's state. Responses are read with
/// <see cref="HttpCompletionOption.ResponseHeadersRead"/> and deserialized directly from the network stream.
/// </remarks>
internal sealed class DiscloudConnection
{
    private const string TokenHeader = "api-token";
    private const string JsonMediaType = "application/json";

    private static readonly string UserAgent = BuildUserAgent();

    private readonly HttpClient _httpClient;
    private readonly Uri _baseAddress;
    private readonly string _token;

    public DiscloudConnection(HttpClient httpClient, DiscloudClientOptions options)
    {
        _httpClient = httpClient;
        _token = options.Token!;

        var baseAddress = options.BaseAddress.AbsoluteUri;
        _baseAddress = baseAddress.EndsWith('/') ? options.BaseAddress : new Uri(baseAddress + "/");
    }

    /// <summary>Sends a request without a body and deserializes the response.</summary>
    public Task<TResponse> SendAsync<TResponse>(
        HttpMethod method,
        string path,
        JsonTypeInfo<TResponse> responseType,
        CancellationToken cancellationToken)
        where TResponse : DiscloudResponse
        => SendAsync(method, path, content: null, responseType, cancellationToken);

    /// <summary>Serializes <paramref name="body"/> as JSON, sends it and deserializes the response.</summary>
    public Task<TResponse> SendJsonAsync<TRequest, TResponse>(
        HttpMethod method,
        string path,
        TRequest body,
        JsonTypeInfo<TRequest> requestType,
        JsonTypeInfo<TResponse> responseType,
        CancellationToken cancellationToken)
        where TResponse : DiscloudResponse
        => SendAsync(method, path, CreateJsonContent(body, requestType), responseType, cancellationToken);

    /// <summary>Sends a request with an arbitrary body and deserializes the response.</summary>
    public Task<TResponse> SendAsync<TResponse>(
        HttpMethod method,
        string path,
        HttpContent? content,
        JsonTypeInfo<TResponse> responseType,
        CancellationToken cancellationToken)
        where TResponse : DiscloudResponse
        => SendCoreAsync(
            method,
            path,
            content,
            (response, ct) => ReadResponseAsync(response, responseType, method, path, ct),
            cancellationToken);

    /// <summary>
    /// Sends a request and lets <paramref name="readSuccess"/> read a successful response, for routes whose body
    /// shape depends on the status code. Use <see cref="ReadResponseAsync{T}"/> inside the callback.
    /// </summary>
    public async Task<TResult> SendCoreAsync<TResult>(
        HttpMethod method,
        string path,
        HttpContent? content,
        Func<HttpResponseMessage, CancellationToken, Task<TResult>> readSuccess,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            content?.Dispose();
            cancellationToken.ThrowIfCancellationRequested();
        }

        using var request = new HttpRequestMessage(method, new Uri(_baseAddress, path));
        request.Content = content;
        request.Headers.TryAddWithoutValidation(TokenHeader, _token);
        request.Headers.TryAddWithoutValidation("Accept", JsonMediaType);
        request.Headers.TryAddWithoutValidation("User-Agent", UserAgent);

        using var response = await SendRequestAsync(request, method, path, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw await CreateApiExceptionAsync(response, method, path, cancellationToken).ConfigureAwait(false);
        }

        try
        {
            return await readSuccess(response, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is HttpRequestException or IOException && !cancellationToken.IsCancellationRequested)
        {
            throw new DiscloudNetworkException($"The connection failed while reading the response of {method} {path}.", ex)
            {
                RequestMethod = method.Method,
                RequestPath = path,
            };
        }
    }

    /// <summary>
    /// Reads a successful response as <typeparamref name="T"/>, raising a <see cref="DiscloudApiException"/> when the
    /// body reports <c>"status": "error"</c>.
    /// </summary>
    public static async Task<T> ReadResponseAsync<T>(
        HttpResponseMessage response,
        JsonTypeInfo<T> typeInfo,
        HttpMethod method,
        string path,
        CancellationToken cancellationToken)
        where T : DiscloudResponse
    {
        if (response.Content.Headers.ContentLength == 0)
        {
            // An empty success body is acceptable for operations that only report an outcome.
            if (typeof(T) == typeof(DiscloudResponse))
            {
                return (T)new DiscloudResponse();
            }

            throw InvalidResponse(response.StatusCode, method, path, "The response body was empty.", inner: null);
        }

        T? result;
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        await using (stream.ConfigureAwait(false))
        {
            try
            {
                result = await JsonSerializer.DeserializeAsync(stream, typeInfo, cancellationToken).ConfigureAwait(false);
            }
            catch (JsonException ex)
            {
                throw InvalidResponse(response.StatusCode, method, path, "The response body was not valid JSON for the expected type.", ex);
            }
        }

        if (result is null)
        {
            throw InvalidResponse(response.StatusCode, method, path, "The response body was null.", inner: null);
        }

        if (IsErrorStatus(result.Status))
        {
            throw DiscloudApiException.Create(
                response.StatusCode,
                result.Message,
                result.StatusCode?.ToString(CultureInfo.InvariantCulture),
                errorData: null,
                retryAfter: null,
                method.Method,
                path);
        }

        return result;
    }

    /// <summary>Returns <paramref name="value"/>, or throws when the API omitted a property the operation returns.</summary>
    public static T Require<T>(T? value, string propertyName)
        where T : class
        => value ?? throw MissingProperty(propertyName);

    /// <inheritdoc cref="Require{T}(T, string)"/>
    public static T Require<T>(T? value, string propertyName)
        where T : struct
        => value ?? throw MissingProperty(propertyName);

    /// <summary>Returns the only item of a single-resource response.</summary>
    public static T RequireSingle<T>(IReadOnlyList<T>? values, string propertyName)
        => values is { Count: > 0 } ? values[0] : throw MissingProperty(propertyName);

    private static DiscloudInvalidResponseException MissingProperty(string propertyName)
        => new($"The Discloud API response did not contain the expected '{propertyName}' data.");

    /// <summary>Serializes <paramref name="body"/> into a JSON request body.</summary>
    public static HttpContent CreateJsonContent<T>(T body, JsonTypeInfo<T> typeInfo)
    {
        // Request bodies are small; buffering them provides a Content-Length header instead of chunked encoding.
        var content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(body, typeInfo));
        content.Headers.TryAddWithoutValidation("Content-Type", JsonMediaType);
        return content;
    }

    private async Task<HttpResponseMessage> SendRequestAsync(
        HttpRequestMessage request,
        HttpMethod method,
        string path,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (TaskCanceledException ex)
        {
            // HttpClient reports its own timeout as a cancellation that the caller did not request.
            throw new DiscloudNetworkException($"The request {method} {path} timed out.", ex.InnerException as TimeoutException ?? (Exception)ex)
            {
                RequestMethod = method.Method,
                RequestPath = path,
            };
        }
        catch (HttpRequestException ex)
        {
            throw new DiscloudNetworkException($"The request {method} {path} could not reach the Discloud API.", ex)
            {
                RequestMethod = method.Method,
                RequestPath = path,
            };
        }
    }

    private static async Task<DiscloudApiException> CreateApiExceptionAsync(
        HttpResponseMessage response,
        HttpMethod method,
        string path,
        CancellationToken cancellationToken)
    {
        ApiErrorResponse? error = null;
        if (response.Content.Headers.ContentLength != 0)
        {
            try
            {
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                await using (stream.ConfigureAwait(false))
                {
                    error = await JsonSerializer.DeserializeAsync(stream, DiscloudJsonContext.Default.ApiErrorResponse, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
            catch (JsonException)
            {
                // Not a Discloud error body (for example an HTML page from a proxy). The status code is still reported;
                // the raw body is deliberately not exposed.
            }
            catch (Exception ex) when (ex is HttpRequestException or IOException && !cancellationToken.IsCancellationRequested)
            {
                // The status code is enough to report the failure.
            }
        }

        var errorCode = error?.Code ?? error?.StatusCode?.ToString(CultureInfo.InvariantCulture);
        return DiscloudApiException.Create(
            response.StatusCode,
            error?.Message,
            errorCode,
            error?.Data,
            GetRetryAfter(response),
            method.Method,
            path);
    }

    private static TimeSpan? GetRetryAfter(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.TooManyRequests || response.Headers.RetryAfter is not { } retryAfter)
        {
            return null;
        }

        if (retryAfter.Delta is { } delta)
        {
            return delta;
        }

        if (retryAfter.Date is { } date)
        {
            var wait = date - DateTimeOffset.UtcNow;
            return wait > TimeSpan.Zero ? wait : TimeSpan.Zero;
        }

        return null;
    }

    private static bool IsErrorStatus(string? status)
        => string.Equals(status, "error", StringComparison.OrdinalIgnoreCase);

    private static DiscloudInvalidResponseException InvalidResponse(
        HttpStatusCode statusCode,
        HttpMethod method,
        string path,
        string reason,
        Exception? inner)
        => new($"Discloud API request {method} {path} returned an unexpected response: {reason}", inner)
        {
            StatusCode = statusCode,
            RequestMethod = method.Method,
            RequestPath = path,
        };

    private static string BuildUserAgent()
    {
        var assembly = typeof(DiscloudConnection).Assembly;
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                      ?? assembly.GetName().Version?.ToString()
                      ?? "0.0.0";

        // Drop the source revision suffix (for example "+abc123") added by SourceLink.
        var plus = version.IndexOf('+', StringComparison.Ordinal);
        if (plus >= 0)
        {
            version = version[..plus];
        }

        return $"Discloud.Net/{version} (+https://github.com/nathancmiguel/Discloud.NET)";
    }
}
